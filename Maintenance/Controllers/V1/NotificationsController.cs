using Maintenance.Application.Features.RequestsComplanit.Commands;
using Maintenance.Application.Features.RequestsComplanit.Queries;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Notifications;
using Maintenance.Domain.Entities.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public long _loggedInUserId;

        public NotificationsController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet("GetNotificationByUserId")]
        public async Task<ResponseDTO> GetNotificationQuery(int pageNumber, int pageSize,long UserId)
        {
            return await _mediator.Send(new GetNotificationQuery()
            {
                PaginatedInputModel = new Application.Helpers.Paginations.PaginatedInputModel()
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,

                },
                UserId= UserId
            });
        }

      

        [HttpPost("PostReadNotification")]

        public async Task<ResponseDTO> PostReadNotificationCommand([FromBody] PostReadNotificationCommand command)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseDTO()
                {
                    StatusEnum = StatusEnum.FailedToSave,
                    Message = "error"
                };
            }
            return await _mediator.Send(command);
        }
        [HttpPost("ReadNotification")]

        public async Task<ResponseDTO> ReadNotificationCommand([FromBody] ReadNotificationCommand command)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseDTO()
                {
                    StatusEnum = StatusEnum.FailedToSave,
                    Message = "error"
                };
            }
            return await _mediator.Send(command);
        }
        [HttpPost("PostNotification")]
        [AllowAnonymous]
        public async Task PostNotificationAsync([FromBody] string Token)
        {
            var tokken = "dCLEhqEiQ26fYT1S6l6GLA:APA91bE81TrIoumpW9ZhE9tx6OmWL5qgmpVlenFqnFODlBX-vDmFLlOQkJLUWrGOIM4jGD2ADHi30Jn8VFdLhJQiA8FraP3QOTQJ5jfq15jVvK6KgAxqzsLjsgbjTGg3QzAUjVmyj99v";
            var notfication = new NotificationDto()
            {
                Body = "AAAAAAAAAAAAAAAAAAAAAAHHHHHHHHHHHHHHHHHHHHHHHHMMMMMMMMMMMMMMMMMMMMMMMMEEEEEEEEEEEEEEDDDDDDd",
                Title="SSSSSSSSSSHHHHHHHHHHHHHRRRRRRRRRR",
                
            };
            await NotificationHelper.FCMNotify(notfication, tokken);
        }
    }
}
