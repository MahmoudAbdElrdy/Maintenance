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
    public class ReadNotificationCommand : IRequest<ResponseDTO>
    {

        public long? NotificationId { get; set; }      
        
        class PostComplanitHistory : IRequestHandler<ReadNotificationCommand, ResponseDTO>
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
            public async Task<ResponseDTO> Handle(ReadNotificationCommand request, CancellationToken cancellationToken)
            {
                try
                {
                  
                    var item = await _NotificationRepository.GetFirstAsync(c => c.Id == request.NotificationId);
                   
                        item.UpdatedOn = DateTime.Now;
                        item.ReadDate = DateTime.Now;
                        item.Read = true;

                  _NotificationRepository.Update(item);
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
