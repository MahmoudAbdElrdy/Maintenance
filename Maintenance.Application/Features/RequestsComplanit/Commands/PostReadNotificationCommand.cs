using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Features.Account.Commands.Login;
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
    public class PostReadNotificationCommand : IRequest<ResponseDTO>
    {

        public long? NotificationId { get; set; }      
        public long? RequestComplanitId { get; set; }
        public long? ComplanitHistoryId { get; set; } 
        public NotificationState NotificationState { get; set; }
        public ComplanitStatus? ComplanitStatus { get; set; }
        class PostComplanitHistory : IRequestHandler<PostReadNotificationCommand, ResponseDTO>
        {
            private readonly IGRepository<ComplanitHistory> _ComplanitHistoryRepository;
            private readonly IGRepository<RequestComplanit> _RequestComplanitRepository;
            private readonly IGRepository<Notification> _NotificationRepository;
            private readonly ILogger<PostReadNotificationCommand> _logger;
            private readonly ResponseDTO _response;
            public readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            private readonly UserManager<User> _userManager;
            private readonly IStringLocalizer<LoginQueryHandler> _localizationProvider;
            public PostComplanitHistory(

               IGRepository<ComplanitHistory> ComplanitHistoryRepository,
               ILogger<PostReadNotificationCommand> logger,
               IAuditService auditService,
               IMapper mapper,
               IGRepository<Notification> NotificationRepository,
               IGRepository<RequestComplanit> RequestComplanitRepository,
                  UserManager<User> userManager,
                IStringLocalizer<LoginQueryHandler> localizationProvider
           )
            {
                _ComplanitHistoryRepository = ComplanitHistoryRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;
                _NotificationRepository = NotificationRepository;
                _RequestComplanitRepository = RequestComplanitRepository;
                _userManager = userManager;
                _localizationProvider = localizationProvider;
            }
            public async Task<ResponseDTO> Handle(PostReadNotificationCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var complanit = await _RequestComplanitRepository.GetFirstAsync(c => c.Id == request.RequestComplanitId);
                  
                    if (
                       ( request.ComplanitStatus==Domain.Enums.ComplanitStatus.TechnicianSuspended ||
                        request.ComplanitStatus==Domain.Enums.ComplanitStatus.TechnicianCanceled 
                        )
                        && request.NotificationState == NotificationState.Approved)
                    {

                        var users = await _userManager.Users.Where(x => (x.UserType == UserType.Owner
                                         || x.UserType == UserType.Consultant ||x.Id==complanit.UpdatedBy) && x.State == State.NotDeleted).ToListAsync();

                        foreach (var item in users)

                        {
                            var notfication = new Notification()
                            {
                                CreatedBy = _auditService.UserId,

                                CreatedOn = DateTime.Now,

                                State = Domain.Enums.State.NotDeleted,

                                From = _auditService.UserId,

                                NotificationState = NotificationState.New,

                                BodyAr = _localizationProvider[Enum.GetName(typeof(Domain.Enums.ComplanitStatus), request.ComplanitStatus), "ar"],

                                BodyEn = _localizationProvider[Enum.GetName(typeof(Domain.Enums.ComplanitStatus), request.ComplanitStatus), "en"],

                                SubjectAr =complanit.Code,

                                SubjectEn = complanit.Code,

                                To = item.Id,

                                Read = false,

                                Type = NotificationType.Message
                            };


                           // notfication.ComplanitHistory = complanitHistory;

                            await _NotificationRepository.AddAsync(notfication);
                            string body = "";
                            if (request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianSuspended)
                            {
                                body = _localizationProvider["ToTechnicianSuspendedApproved"] + " " + complanit.Code;
                            }
                            else if (request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianCanceled)
                            {
                                body = _localizationProvider["ToTechnicianCanceledApproved"] + " " + complanit.Code;
                            }

                            var notificationDto = new NotificationDto()
                            {
                                Title = complanit.Code,
                                Body = body
                            };

                            await NotificationHelper.FCMNotify(notificationDto, item.Token);

                        }
                        complanit.ComplanitStatus = request.ComplanitStatus;
                       
                        complanit.UpdatedOn = DateTime.Now;
                        complanit.UpdatedBy = _auditService.UserId;
                        _RequestComplanitRepository.Update(complanit);


                    }
                    if (
                       (request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianSuspended ||
                        request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianCanceled 
                      )
                       && request.NotificationState == NotificationState.Rejected)
                    {
                       // var itemHostory=await _ComplanitHistoryRepository.GetFirstAsync(c => c.Id == request.RequestComplanitId);
                        var complanitHistory = new ComplanitHistory()
                        {
                            CreatedBy = _auditService.UserId,
                            CreatedOn = DateTime.Now,
                            State = Domain.Enums.State.NotDeleted,
                            Description = request.ComplanitStatus.ToString(),
                            ComplanitStatus = request.ComplanitStatus,//TechnicianClosed no message
                            RequestComplanitId = request.RequestComplanitId
                        };

                        complanitHistory.ComplanitStatus = Domain.Enums.ComplanitStatus.Submitted;

                       // complanitHistory.UpdatedOn = DateTime.Now;

                        complanit.UpdatedOn = DateTime.Now;

                        _RequestComplanitRepository.Update(complanit);
                      
                        var users = await _userManager.Users.Where(x => (x.UserType == UserType.Owner
                                     || x.UserType == UserType.Consultant || x.Id == complanit.UpdatedBy) && x.State == State.NotDeleted).ToListAsync();

                        foreach (var item in users)

                        {
                            var notfication = new Notification()
                            {
                                CreatedBy = _auditService.UserId,

                                CreatedOn = DateTime.Now,

                                State = Domain.Enums.State.NotDeleted,

                                From = _auditService.UserId,

                                NotificationState = NotificationState.New,

                                BodyAr = _localizationProvider[Enum.GetName(typeof(Domain.Enums.ComplanitStatus), request.ComplanitStatus), "ar"],

                                BodyEn = _localizationProvider[Enum.GetName(typeof(Domain.Enums.ComplanitStatus), request.ComplanitStatus), "en"],

                                SubjectAr = complanit.Code,

                                SubjectEn = complanit.Code,

                                To = item.Id,

                                Read = false,

                                Type = NotificationType.Message
                            };


                            notfication.ComplanitHistory = complanitHistory;
                           // await _ComplanitHistoryRepository.AddAsync(complanitHistory);

                            await _NotificationRepository.AddAsync(notfication);
                            string body = "";
                            if (request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianSuspended)
                            {
                                body = _localizationProvider["ToTechnicianSuspendedRejected"] + " " + complanit.Code;
                            }
                            else if (request.ComplanitStatus == Domain.Enums.ComplanitStatus.TechnicianCanceled)
                            {
                                body = _localizationProvider["ToTechnicianCanceledRejected"] + " " + complanit.Code;
                            }

                            var notificationDto = new NotificationDto()
                            {
                                Title = complanit.Code,
                                Body = body
                            };

                            await NotificationHelper.FCMNotify(notificationDto, item.Token);

                        }
                    }





                    var NotficationList = await _NotificationRepository.GetAll(c => c.ComplanitHistoryId == request.ComplanitHistoryId).ToListAsync();
                    foreach(var item in NotficationList)
                    {
                        item.UpdatedOn = DateTime.Now;
                        item.ReadDate = DateTime.Now;
                        item.NotificationState = request.NotificationState;
                        item.Read = true;

                        _NotificationRepository.Update(item);
                        _NotificationRepository.Save();

                    }

                    //var Notification = await _NotificationRepository.GetFirstAsync(c => c.Id == request.NotificationId);

                  

                    //Notification.UpdatedOn = DateTime.Now;
                    //Notification.ReadDate = DateTime.Now;
                    //Notification.NotificationState = request.NotificationState;
                    //Notification.Read = true;
                 
                    //_NotificationRepository.Update(Notification);

                    _NotificationRepository.Save();

                    _response.StatusEnum = StatusEnum.SavedSuccessfully;
                    _response.Message = "Successfully";
                    _response.Result = null;
                    return _response;
                }
                catch (Exception ex)
                {
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
