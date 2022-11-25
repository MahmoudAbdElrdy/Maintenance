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
        [HttpPost("PostNotification")]

        public async Task PostNotificationAsync([FromBody] string Token)
        {
            var tokken = "fwAE0Y95QLOkhl2Gw0Hf9s:APA91bFKjvu9X-dlIETVAdW90MvXMiX9SHFV3Wzso1CG7IaRpJ8OZlei-ksx6hQ2yOvqJJfpeVUm5IXz-uABbrmYbkRZtjYs8fposHVkv4vyZoMYoM6F2XS3b76kDrypTmT5Gak2R7sy";
            var notfication = new NotificationDto()
            {
                Body = "AAAAAAAAAAAAAAAAAAAAAAHHHHHHHHHHHHHHHHHHHHHHHHMMMMMMMMMMMMMMMMMMMMMMMMEEEEEEEEEEEEEEDDDDDDd",
                Title="SSSSSSSSSSHHHHHHHHHHHHHRRRRRRRRRR",
                
            };
            await NotificationHelper.FCMNotify(notfication, Token);
        }
    }
}
