using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Features.Account.Commands.Login;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Notifications;
using Maintenance.Application.Helpers.SendSms;
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
    public class PostApproveComplanitHistoryCommand : IRequest<ResponseDTO>
    {
        public string? Description { get; set; }
        public string[]? AttachmentsComplanitHistory { get; set; }
        public ComplanitStatus? ComplanitStatus { get; set; }
        public long? RequestComplanitId { get; set; }
        //public string? Code { get; set; }
        class PostRequestComplanit : IRequestHandler<PostApproveComplanitHistoryCommand, ResponseDTO>
        {
            private readonly IGRepository<ComplanitHistory> _ComplanitHistoryRepository;
            private readonly IGRepository<RequestComplanit> _RequestComplanitRepository; 
            private readonly IGRepository<Notification> _NotificationRepository;
            private readonly IGRepository<RequestComplanitNotification> _RequestComplanitNotificationRepository;
            private readonly ILogger<PostApproveComplanitHistoryCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            private readonly UserManager<User> _userManager;
            private readonly IStringLocalizer<LoginQueryHandler> _localizationProvider;
            public PostRequestComplanit(

                IGRepository<ComplanitHistory> ComplanitHistoryRepository,
                ILogger<PostApproveComplanitHistoryCommand> logger,
                IAuditService auditService,
                IMapper mapper,
                IGRepository<Notification> NotificationRepository,
                UserManager<User> userManager,
                IStringLocalizer<LoginQueryHandler> localizationProvider,
                IGRepository<RequestComplanit> RequestComplanitRepository,
                IGRepository<RequestComplanitNotification> RequestComplanitNotificationRepository
            )
            {
                _ComplanitHistoryRepository = ComplanitHistoryRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;
                _NotificationRepository = NotificationRepository;
                _userManager = userManager;
                _localizationProvider = localizationProvider;
                _RequestComplanitRepository = RequestComplanitRepository;
                _RequestComplanitNotificationRepository = RequestComplanitNotificationRepository;
            }
            public async Task<ResponseDTO> Handle(PostApproveComplanitHistoryCommand request, CancellationToken cancellationToken)
            {
                try
                {
                   
                     var complaintSataus =await _ComplanitHistoryRepository.GetAll(c => c.RequestComplanitId == request.RequestComplanitId).ToListAsync();
                   
                    var complaint = await _RequestComplanitRepository.GetFirstAsync(c => c.Id == request.RequestComplanitId);


                    //  if (complaintSataus.OrderBy(c => c.CreatedBy).Any(c => c.ComplanitStatus == request.ComplanitStatus))
                    if (complaintSataus!=null && complaintSataus.Count>0)
                    {
                        //_response.StatusEnum = StatusEnum.Failed;
                        //_response.Message = _localizationProvider["This Status Send Befor"];
                        //_response.Result = null;
                        //return _response;
                        var idsHistory = complaintSataus.Select(c => c.Id).ToList();

                        var NotficationList = await _NotificationRepository.GetAll(c => idsHistory.Contains((long)c.ComplanitHistoryId) && c.Read==false).ToListAsync();
                      
                        foreach (var item in NotficationList)
                        {
                            item.UpdatedOn = DateTime.Now;
                            item.ReadDate = DateTime.Now;
                            item.NotificationState = NotificationState.New;
                            item.Read = true;

                            _NotificationRepository.Update(item);
                            _NotificationRepository.Save();

                        }

                    }

                    //if (
                    //    complaintSataus.Any(x => x.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianCanceled)
                    //    ||
                    //    complaintSataus.Any(x => x.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianSuspended ) ||
                    //    complaintSataus.Any(x => x.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianClosed)


                    //    )
                    //{
                    //    _response.StatusEnum = StatusEnum.Failed;
                    //    _response.Message = _localizationProvider["ApproveOrRejectedStatus"];
                    //    _response.Result = null;
                    //    return _response;
                    //}

                        var complanitHistory = new ComplanitHistory()
                        {
                            CreatedBy = _auditService.UserId,
                            CreatedOn = DateTime.Now,
                            State = Domain.Enums.State.NotDeleted,
                            Description = request.Description,
                            ComplanitStatus = request.ComplanitStatus,//TechnicianClosed no message
                            RequestComplanitId = request.RequestComplanitId
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

                 
                   
                    //Domain.Enums.ComplanitStatus.TechnicianAssigned
                    //The owner and the consultant will recieve a notification contains
                  

                    if (  request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianAssigned  )
                    {


                        var users = await _userManager.Users.Where(x => (x.UserType == UserType.Owner
                                         || x.UserType == UserType.Consultant) && x.State == State.NotDeleted).ToListAsync();

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
                               
                                To=item.Id,
                             
                                Read=false,
                              
                               Type = NotificationType.Message
                          };
                          

                            notfication.ComplanitHistory = complanitHistory;

                            await _NotificationRepository.AddAsync(notfication);
                            var notificationDto = new NotificationDto()
                            {
                                Title=complaint.Code,
                                Body= _localizationProvider["ResponsesToComplaint"]
                            };

                             await NotificationHelper.FCMNotify(notificationDto, item.Token);
                            
                        }
                      
                        complaint.UpdatedOn = DateTime.Now;
                        complaint.UpdatedBy = _auditService.UserId;
                        complaint.ComplanitStatus = request.ComplanitStatus;

                        _RequestComplanitRepository.Update(complaint);
                    }
                    if (  request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianSuspended
                        || request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianCanceled
                        || request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianClosed
                        )
                    {
                        var users = await _userManager.Users.Where(x => (x.UserType == UserType.Owner
                                   || x.UserType == UserType.Consultant) && x.State == State.NotDeleted).ToListAsync();
                        foreach (var item in users)
                        {
                            var notfication = new Notification()
                            {
                                CreatedBy = _auditService.UserId,

                                CreatedOn = DateTime.Now,

                                State = Domain.Enums.State.NotDeleted,

                                From = _auditService.UserId,

                                NotificationState = NotificationState.New,

                                SubjectAr = _localizationProvider[Enum.GetName(typeof(Domain.Enums.ComplanitStatus), request.ComplanitStatus), "ar"],

                                SubjectEn = _localizationProvider[Enum.GetName(typeof(Domain.Enums.ComplanitStatus), request.ComplanitStatus), "en"],

                                BodyAr = request.Description,

                                BodyEn = request.Description,

                                Read = false,

                                To = item.Id,

                                Type = NotificationType.RequestComplanit

                                
                            };
                            complaint.UpdatedOn = DateTime.Now;
                            complaint.UpdatedBy = _auditService.UserId;

                            _RequestComplanitRepository.Update(complaint);
                            notfication.ComplanitHistory = complanitHistory;
                            await _NotificationRepository.AddAsync(notfication);
                            var notificationDto = new NotificationDto()
                            {
                                Title = complaint.Code,
                                Body = _localizationProvider["ResponsesToComplaint"]
                            };

                            await NotificationHelper.FCMNotify(notificationDto, item.Token);
                        }
                    }
                    if (request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianDone)
                    {
                        
                        
                       
                        var clientUser = await _userManager.Users.Where(x => x.Id == complaint.CreatedBy).FirstOrDefaultAsync();

                        complaint.CodeSms= SendSMS.GenerateCode();
                        complaint.UpdatedOn = DateTime.Now;
                        complaint.UpdatedBy = _auditService.UserId;
                        await _ComplanitHistoryRepository.AddAsync(complanitHistory);
                      
                        _RequestComplanitRepository.Update(complaint);
                        //var res = SendSMS.SendMessageUnifonic(meass + " : " + clientUser.Code, clientUser.PhoneNumber);
                        //if (res == -1)
                        //{
                        //    _response.Message = _localizationProvider["ProplemSendCode"];

                        //    _response.StatusEnum = StatusEnum.Failed;
                        //    return _response;
                        //}
                        _ComplanitHistoryRepository.Save();

                        _response.StatusEnum = StatusEnum.SavedSuccessfully;
                        _response.Message = _localizationProvider["Send Code To Technician"];
                        _response.Result =
                            new { 
                            CodeSms= complaint.CodeSms ,
                            RequestComplanitId=request.RequestComplanitId
                            } ;
                        return _response;
                    }


                    _ComplanitHistoryRepository.Save();

                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = _localizationProvider["AddedSuccessfully"];
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
