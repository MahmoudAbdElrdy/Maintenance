using AuthDomain.Entities.Auth;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.SendSms;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Maintenance.Application.Features.Users.Commands.EditUserCommand
{
    public class EditUserCommand : IRequest<ResponseDTO>
    {
        public long Id { get; set; }
        public string? FullName { get; set; }
        public UserType UserType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IdentityNumber { get; set; }
        public long? OfficeId { get; set; }
        public long? RegionId { get; set; }
    }
    class EditUserCommandHandler : IRequestHandler<EditUserCommand, ResponseDTO>
    {
        private readonly UserManager<User> _userManager;
        private readonly ResponseDTO _responseDTO;
        private readonly IAuditService _auditService;
        private readonly IStringLocalizer<EditUserCommand> _stringLocalizer;
        public EditUserCommandHandler(UserManager<User> userManager,  IStringLocalizer<EditUserCommand> stringLocalizer, IAuditService auditService)
        {
            _userManager = userManager;
            _responseDTO = new ResponseDTO();
            _auditService = auditService;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<ResponseDTO> Handle(EditUserCommand request, CancellationToken cancellationToken)
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

                checkExsit.UserName = request.IdentityNumber;
                checkExsit.Email = request.PhoneNumber + "@Gamil.com";
                checkExsit.FullName = request.FullName;
                checkExsit.PhoneNumber = request.PhoneNumber;
                checkExsit.NormalizedEmail = request.PhoneNumber + "@Gamil.com";
                checkExsit.NormalizedUserName = request.IdentityNumber;
                checkExsit.UpdatedOn = DateTime.Now;
                checkExsit.State = State.NotDeleted;
                checkExsit.IdentityNumber = request.IdentityNumber;
                checkExsit.UserType = request.UserType;
                checkExsit.OfficeId = request.OfficeId;
                checkExsit.RegionId = request.RegionId;

                var result = await _userManager.UpdateAsync(checkExsit);
                if (!result.Succeeded)
                {
                    _responseDTO.Result = null;
                    _responseDTO.StatusEnum = StatusEnum.Exception;
                    _responseDTO.Message = "anErrorOccurredPleaseContactSystemAdministrator";

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

