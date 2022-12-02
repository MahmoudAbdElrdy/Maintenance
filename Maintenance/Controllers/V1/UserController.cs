using Maintenance.Application.Auth.Client.Command;
using Maintenance.Application.Auth.UpdateToken.Command;
using Maintenance.Application.Features.Users.Commands.AddUserCommand;
using Maintenance.Application.Features.Users.Commands.DeleteUserCommand;
using Maintenance.Application.Features.Users.Queries.CheckCodeApplyJob;
using Maintenance.Application.Features.Users.Queries.GenerateCodeApplyJob;
using Maintenance.Application.Features.Users.Queries.GetUserByIdQuery;
using Maintenance.Application.Features.Users.Queries.GetUsersQuery;
using Maintenance.Application.Helper;
using Maintenance.Domain.Enums;
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
        [HttpPost]
       // [AllowAnonymous]
        [Route("UpdateToken")]
        public async Task<ResponseDTO> UpdateToken([FromBody] UpdateTokenCommand command) 
        {
            return await _mediator.Send(command);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GenerateCodeApplyJob")]
        public async Task<ResponseDTO> GenerateCodeApplyJob([FromBody] GetGenerateCodeApplyJobQuery command)
        {
            return await _mediator.Send(command);
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("CheckCodeApplyJob")]
        public async Task<ResponseDTO> CheckCodeApplyJob(string nationalId, string code)
        {
            return await _mediator.Send(new CheckCodeApplyJobQuery()
            {
                NationalId = nationalId,
                Code = code
            });
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetUsers")]
        public async Task<ResponseDTO> GetUsers(UserType userType,string Name)
        {
            return await _mediator.Send(new GetUsersQuery()
            {
                Name = Name,
                UserType = userType
            });
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("GetUserById/{Id}")]
        public async Task<ResponseDTO> GetUserById(long Id)
        {
            return await _mediator.Send(new GetUserByIdQuery()
            {
                Id = Id
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("AddUser")]
        public async Task<ResponseDTO> AddUser([FromBody] AddUserCommand command)
        {
            return await _mediator.Send(command);
        }


        [HttpDelete]
        [AllowAnonymous]
        [Route("deleteUser/{id}")]
        public async Task<ResponseDTO> DeleteUser(long id)
        {
            return await _mediator.Send(new DeleteUserCommand() { Id = id } );
        }


    }
}
