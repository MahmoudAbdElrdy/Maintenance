using Maintenance.Application.Features.Categories.Commands;
using Maintenance.Application.Features.Categories.Queries;
using Maintenance.Application.Features.RequestsComplanit.Commands;
using Maintenance.Application.Features.RequestsComplanit.Queries;
using Maintenance.Application.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
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
    }
}
