using AuthDomain.Entities.Auth;
using AutoMapper;
using FCM.Net;
using Maintenance.Application.GenericRepo;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Paginations;
using Maintenance.Application.Helpers.QueryableExtensions;
using Maintenance.Domain.Entities.Auth;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notification = Maintenance.Domain.Entities.Auth.Notification;

namespace Maintenance.Application.Features.RequestsComplanit.Queries
{
    public class GetNotificationQuery : IRequest<ResponseDTO>
    {
        public GetNotificationQuery()
        {
            PaginatedInputModel = new PaginatedInputModel();
        }
        public long? UserId { get; set; }
       

        public PaginatedInputModel PaginatedInputModel { get; set; }
        class GetAllNotificationUser : IRequestHandler<GetNotificationQuery, ResponseDTO>
        {

            private readonly IGRepository<RequestComplanit> _RequestComplanitRepository;
            private readonly IGRepository<CheckListRequest> _CheckListRequestRepository;
            private readonly IGRepository<Notification> _NotficationRepository;
            private readonly IGRepository<RequestComplanitNotification> _RequestComplanitNotificationRepository;

            private readonly IGRepository<User> _userRepository;
            private readonly ILogger<GetNotificationQuery> _logger;
            private readonly ResponseDTO _response;
            private readonly IAuditService _auditService;
            private readonly IMapper _mapper;
            public GetAllNotificationUser(

                ILogger<GetNotificationQuery> logger, IMapper mapper,
                IGRepository<User> userRepository,
                IAuditService auditService,

                IGRepository<RequestComplanit> RequestComplanitRepository,
                IGRepository<CheckListRequest> CheckListRequestRepository,
                IGRepository<Notification> NotficationRepository,
                IGRepository<RequestComplanitNotification> RequestComplanitNotificationRepository

            )
            {
                _mapper = mapper;

                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _response = new ResponseDTO();
                _userRepository = userRepository;
                _auditService = auditService;

                _RequestComplanitRepository = RequestComplanitRepository;
                _CheckListRequestRepository = CheckListRequestRepository;
                _RequestComplanitNotificationRepository = RequestComplanitNotificationRepository;
                _NotficationRepository = NotficationRepository;

            }
            public async Task<ResponseDTO> Handle(GetNotificationQuery request, CancellationToken cancellationToken)
            {
                try
                {

                    var user =await _userRepository.GetFirstAsync(c => c.Id == request.UserId);

                    var resx = await _NotficationRepository
                        .GetAllIncluding(c=>c.ComplanitHistory.AttachmentComplanitHistory).
                        Include(c=>c.ComplanitHistory.RequestComplanit)
           
                        .Where(x=>x.To==request.UserId &&x.State==State.NotDeleted)
                        .WhereIf(user.UserType == UserType.Owner || user.UserType == UserType.Client, x=> x.Type== NotificationType.Message)
                        .WhereIf(user.UserType == UserType.Consultant, x=> (x.Type== NotificationType.Message ) || ( x.Type == NotificationType.RequestComplanit && x.Read==false))
                        .Select(c=>new
                        {
                            Title = c.ComplanitHistory.RequestComplanit.Code,
                            Description = c.ComplanitHistory.Description,
                            AttachmentComplanitHistory = c.ComplanitHistory.AttachmentComplanitHistory.Select(m => m.Path),
                            NotificationType = c.Type,
                            NotificationId =c.Id,
                            NotificationState = (int) c.NotificationState,
                            Body=_auditService.UserLanguage=="ar"? c.BodyEn:c.BodyEn,
                            Subject = _auditService.UserLanguage=="ar"? c.SubjectAr:c.SubjectEn,
                            ComplanitStatus =(int) c.ComplanitHistory.ComplanitStatus,
                            IsRead= c.Read,
                            
                            ComplanitHistoryId = c.ComplanitHistoryId,
                         
                            RequestComplanitId = c.ComplanitHistory.RequestComplanitId,
                             

                        })
                        .ToListAsync();

                    var paginatedObjs = await PaginationUtility.Paging(request.PaginatedInputModel, resx);

                    _response.setPaginationData(paginatedObjs);
                    _response.Result = paginatedObjs;
                    _response.StatusEnum = StatusEnum.Success;
                    _response.Message = "CategoryComplanitRetrievedSuccessfully";

                    return _response;
                }
                catch (Exception ex)
                {
                    _response.StatusEnum = StatusEnum.Exception;
                    _response.Result = null;
                    _response.Message = (ex != null && ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                    _logger.LogError(ex, ex.Message, (ex != null && ex.InnerException != null ? ex.InnerException.Message : ""));

                    return _response;
                }
            }

        }
    }
    
}
