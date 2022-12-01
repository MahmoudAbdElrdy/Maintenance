using AuthDomain.Entities.Auth;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.SendSms;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Maintenance.Application.Features.Users.Commands.DeleteUserCommand
{
    public class DeleteUserCommand : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
    }
    class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ResponseDTO>
    {
        private readonly UserManager<User> _userManager;
        private readonly ResponseDTO _responseDTO;
        private readonly IAuditService _auditService;
        private readonly IStringLocalizer<DeleteUserCommand> _stringLocalizer;
        public DeleteUserCommandHandler(UserManager<User> userManager,  IStringLocalizer<DeleteUserCommand> stringLocalizer, IAuditService auditService)
        {
            _userManager = userManager;
            _responseDTO = new ResponseDTO();
            _auditService = auditService;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<ResponseDTO> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var checkExsit = await _userManager.Users.Where(x => x.Id == request.Id).FirstOrDefaultAsync();

                if (checkExsit == null)
                {
                    _responseDTO.Result = null;
                    _responseDTO.StatusEnum = StatusEnum.Failed;

                    _responseDTO.Message = _stringLocalizer["UserNotFound"];
                    return _responseDTO;
                }

                checkExsit.State = State.Deleted;

                var result = await _userManager.UpdateAsync(checkExsit);
                if (!result.Succeeded)
                {
                    _responseDTO.Result = null;
                    _responseDTO.StatusEnum = StatusEnum.Exception;
                    _responseDTO.Message = _stringLocalizer["anErrorOccurredPleaseContactSystemAdministrator"];

                    return _responseDTO;
                }

            }
            catch (Exception ex)
            {
                _responseDTO.Result = null;
                _responseDTO.StatusEnum = StatusEnum.Exception;
                _responseDTO.Message = _stringLocalizer["anErrorOccurredPleaseContactSystemAdministrator"];

            }
            
            _responseDTO.Message = "userUpdatedSuccessfully";

            _responseDTO.StatusEnum = StatusEnum.SavedSuccessfully;

            return _responseDTO;


        }

    }
}

