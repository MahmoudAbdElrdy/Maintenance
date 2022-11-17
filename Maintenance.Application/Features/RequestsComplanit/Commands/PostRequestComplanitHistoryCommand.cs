using AutoMapper;
using Maintenance.Application.Features.RequestsComplanit.Dto;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Domain.Entities.Auth;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using MediatR;
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
          
            public PostRequestComplanit(

                IGRepository<ComplanitHistory> RequestComplanitRepository,
                ILogger<PostRequestComplanitHistoryCommand> logger,
                IAuditService auditService,
                IMapper mapper,
                IGRepository<Notification> NotificationRepository
            )
            {
                _RequestComplanitRepository = RequestComplanitRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;
                _NotificationRepository = NotificationRepository;
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


                    if (request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianAssigned)
                    {
                        var notfication = new Notification()
                        {
                            CreatedBy = _auditService.UserId,
                            CreatedOn = DateTime.Now,
                            State = Domain.Enums.State.NotDeleted,
                            From = _auditService.UserId,
                            NotificationState=NotificationState.New,
                            SubjectAr= "TechnicianAssigned",
                            SubjectEn= "TechnicianAssigned",
                            BodyAr=request.Description,
                            BodyEn=request.Description,
                            
                        };
                        //send notfication name and datials tech
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
