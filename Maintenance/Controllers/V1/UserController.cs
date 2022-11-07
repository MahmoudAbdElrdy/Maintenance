using Maintenance.Application.Auth.Client.Command;
using Maintenance.Application.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Controllers.V1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class UserController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<ResponseDTO> Register([FromBody] ClientRegisterCommand command)
        {
            return await _mediator.Send(command);
        }
    }
}
