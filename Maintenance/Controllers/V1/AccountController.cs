using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Controllers.V1
{
    [ApiVersion("1.0")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public long _loggedInUserId;

        public AccountController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _httpContextAccessor = httpContextAccessor;
        }
    }
}
