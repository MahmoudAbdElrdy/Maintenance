using AuthDomain.Entities.Auth;
using AutoMapper;
using Infrastructure;
using IronBarCode.Logging;
using Maintenance.Application.Features.Account.Commands.Login;
using Maintenance.Application.Features.Categories.Commands;
using Maintenance.Application.Features.Categories.Dto;
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
            public PostComplanitHistory(

               IGRepository<ComplanitHistory> ComplanitHistoryRepository,
               ILogger<PostReadNotificationCommand> logger,
               IAuditService auditService,
               IMapper mapper,
               IGRepository<Notification> NotificationRepository,
               IGRepository<RequestComplanit> RequestComplanitRepository
           )
            {
                _ComplanitHistoryRepository = ComplanitHistoryRepository;
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _auditService = auditService;
                _response = new ResponseDTO();
                _mapper = mapper;
                _NotificationRepository = NotificationRepository;
                _RequestComplanitRepository = RequestComplanitRepository;
            }
            public async Task<ResponseDTO> Handle(PostReadNotificationCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var complanit = await _RequestComplanitRepository.GetFirstAsync(c => c.Id == request.RequestComplanitId);
                  
                    if (
                        request.ComplanitStatus==Domain.Enums.ComplanitStatus.TechnicianSuspended ||
                        request.ComplanitStatus==Domain.Enums.ComplanitStatus.TechnicianCanceled ||
                        request.ComplanitStatus==Domain.Enums.ComplanitStatus.TechnicianClosed 
                        && request.NotificationState == NotificationState.Approved)
                    {
                        complanit.ComplanitStatus = request.ComplanitStatus;
                       
                        complanit.UpdatedOn = DateTime.Now;
                       
                        _RequestComplanitRepository.Update(complanit);
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
