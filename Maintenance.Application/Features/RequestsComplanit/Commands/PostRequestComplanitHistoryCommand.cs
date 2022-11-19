﻿using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Features.Account.Commands.Login;
using Maintenance.Application.Features.RequestsComplanit.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Notifications;
using Maintenance.Domain.Entities.Auth;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Maintenance.Application.Features.RequestsComplanit.Commands
{
    public class PostRequestComplanitHistoryCommand : IRequest<ResponseDTO>
    {


        public string? Description { get; set; }
        public string[]? AttachmentsComplanitHistory { get; set; }
        public ComplanitStatus? ComplanitStatus { get; set; }
        class PostRequestComplanit : IRequestHandler<PostRequestComplanitHistoryCommand, ResponseDTO>
        {
            private readonly IGRepository<ComplanitHistory> _RequestComplanitRepository;
            private readonly IGRepository<Notification> _NotificationRepository;
            private readonly ILogger<PostRequestComplanitHistoryCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            private readonly UserManager<User> _userManager;
            private readonly IStringLocalizer<LoginQueryHandler> _localizationProvider;
            public PostRequestComplanit(

                IGRepository<ComplanitHistory> RequestComplanitRepository,
                ILogger<PostRequestComplanitHistoryCommand> logger,
                IAuditService auditService,
                IMapper mapper,
                IGRepository<Notification> NotificationRepository,
                UserManager<User> userManager,
                IStringLocalizer<LoginQueryHandler> localizationProvider
            )
            {
                _RequestComplanitRepository = RequestComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;
                _NotificationRepository = NotificationRepository;
                _userManager = userManager;
                _localizationProvider = localizationProvider;
            }
            public async Task<ResponseDTO> Handle(PostRequestComplanitHistoryCommand request, CancellationToken cancellationToken)
            {
                try
                {
                  
                  
                        var complanitHistory = new ComplanitHistory()
                        {
                            CreatedBy = _auditService.UserId,
                            CreatedOn = DateTime.Now,
                            State = Domain.Enums.State.NotDeleted,
                            Description = request.Description,
                            ComplanitStatus = request.ComplanitStatus,
                           
                        };

                        foreach (var item in request.AttachmentsComplanitHistory)
                        {
                            complanitHistory.AttachmentComplanitHistory.Add(new AttachmentComplanitHistory()
                            {
                                Path = item,
                                CreatedBy = _auditService.UserId,
                                CreatedOn = DateTime.Now,
                            });
                        }

                    await _RequestComplanitRepository.AddAsync(complanitHistory);
                   
                    var personalUser = await _userManager.Users.Where(x => x.Id == _auditService.UserId).FirstOrDefaultAsync();
                   
                    if (personalUser == null)
                    {
                      _response.Message = _localizationProvider["UserNotFound"];

                      _response.StatusEnum = StatusEnum.Failed;
                      
                        return _response;
                    }
                    //Domain.Enums.ComplanitStatus.TechnicianAssigned
                    //The owner and the consultant will recieve a notification contains
                    var users = await _userManager.Users.Where(x => x.UserType == UserType.Owner 
                    || x.UserType == UserType.Consultant &&x.State == State.NotDeleted).ToListAsync();

                    if (  request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianAssigned 
                        ||request.ComplanitStatus== Domain.Enums.ComplanitStatus.TechnicianSuspended 
                        || request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianCanceled
                        || request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianDone
                        )
                    {
                        foreach (var item in users)
                      
                       {
                          var notfication = new Notification()
                            {
                                CreatedBy = _auditService.UserId,
                               
                                CreatedOn = DateTime.Now,
                               
                                State = Domain.Enums.State.NotDeleted,
                               
                                From = _auditService.UserId,
                               
                                NotificationState = NotificationState.New,
                               
                                SubjectAr = _localizationProvider[Enum.GetName(typeof(Domain.Enums.ComplanitStatus), request.ComplanitStatus),"ar"],
                              
                                SubjectEn = _localizationProvider[Enum.GetName(typeof(Domain.Enums.ComplanitStatus), request.ComplanitStatus),"en"],
                              
                                BodyAr = request.Description,
                              
                                BodyEn = request.Description,
                               
                                To=item.Id
                            };
                          
                         await NotificationHelper.FCMNotify(notfication, "");
                        
                         await  _NotificationRepository.AddAsync(notfication);
                        }
                    }
                 
                    _RequestComplanitRepository.Save();

                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "RequestComplanitSavedSuccessfully";
                    _response.Result = null;
                    return _response;
                }
                catch (Exception ex)
                {
                  
                        if (request.AttachmentsComplanitHistory.Length > 0)
                        {
                            var folderName = Path.Combine("wwwroot/Uploads/Complanits");

                            foreach (var fileRemove in request.AttachmentsComplanitHistory)
                            {
                                var file = System.IO.Path.Combine(folderName, fileRemove);
                                try
                                {
                                    System.IO.File.Delete(file);
                                }
                                catch { }
                            }


                        }        
                    _response.StatusEnum = StatusEnum.Exception;
                    _response.Result = null;
                    _response.Message = ex != null && ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    _logger.LogError(ex, ex.Message, ex != null && ex.InnerException != null ? ex.InnerException.Message : "");

                    return _response;
                }
            }

        }
    }
}
