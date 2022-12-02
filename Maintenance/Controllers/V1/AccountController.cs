using Maintenance.Application.Auth.VerificationCode.Command;
using Maintenance.Application.Features.Account.Commands.Login;
using Maintenance.Application.Features.Account.Queries.CheckLogin;
using Maintenance.Application.Features.Account.Queries.CheckLoginWeb;
using Maintenance.Application.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Controllers.V1
{
    [ApiVersion("1.0")]
    [ApiController]
    public class AccountController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public long _loggedInUserId;

        public AccountController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _httpContextAccessor = httpContextAccessor;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<ResponseDTO> Login([FromBody] LoginQuery command)
        {
            return await _mediator.Send(command);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("CheckLogin")]
        public async Task<ResponseDTO> CheckLogin([FromBody] CheckLoginQuery command)
        {
            return await _mediator.Send(command);
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("webLogin")]
        public async Task<ResponseDTO> CheckLogin([FromBody] CheckLoginWebQuery command)
        {
            return await _mediator.Send(command);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("VerificationCode")]
        public async Task<ResponseDTO> VerificationCode([FromBody] VerificationCodeCommand command)
        {
            return await _mediator.Send(command);
        }
    }
}
