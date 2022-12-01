using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Auth.Login;
using Maintenance.Application.Features.Account.Commands.Login;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.SendSms;
using Maintenance.Application.Interfaces;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Refit;

namespace Maintenance.Application.Features.Users.Commands.AddUserCommand
{
    public class AddUserCommand : IRequest<ResponseDTO>
    {
        public string? FullName { get; set; }
        public UserType UserType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IdentityNumber { get; set; }
        public string? Password { get; set; }
        public long? OfficeId { get; set; }
        public long? RegionId { get; set; }
    }
    class AddUserCommandHandler : IRequestHandler<AddUserCommand, ResponseDTO>
    {
        private readonly UserManager<User> _userManager;
        private readonly ResponseDTO _responseDTO;
        private readonly IAuditService _auditService;
        private readonly IStringLocalizer<AddUserCommand> _stringLocalizer;
        public AddUserCommandHandler(UserManager<User> userManager,  IStringLocalizer<AddUserCommand> stringLocalizer, IAuditService auditService)
        {
            _userManager = userManager;
            _responseDTO = new ResponseDTO();
            _auditService = auditService;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<ResponseDTO> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User();
            try
            {

                var checkExsit = await _userManager.Users.Where(x => x.UserType == request.UserType &&
                                            x.IdentityNumber == request.IdentityNumber).FirstOrDefaultAsync();

                if (checkExsit != null)
                {
                    _responseDTO.Result = null;
                    _responseDTO.StatusEnum = StatusEnum.Failed;

                    _responseDTO.Message = _stringLocalizer["NationalNumberFoundBefore"];
                    return _responseDTO;
                }
                var phoneExsit = await _userManager.Users.Where(x => x.PhoneNumber == request.PhoneNumber).FirstOrDefaultAsync();

                if (phoneExsit != null)
                {
                    _responseDTO.Result = null;
                    _responseDTO.StatusEnum = StatusEnum.Failed;
                    _responseDTO.Message = _stringLocalizer["PhoneNumberFoundBefore"];

                    return _responseDTO;
                }

                user = new User()
                {
                    UserName = request.IdentityNumber,

                    Email = request.PhoneNumber + "@Gamil.com",

                    FullName = request.FullName,

                    PhoneNumber = request.PhoneNumber,

                    NormalizedEmail = request.PhoneNumber + "@Gamil.com",

                    NormalizedUserName = request.IdentityNumber,

                    CreatedOn = DateTime.Now,

                    State = State.NotDeleted,

                    IdentityNumber = request.IdentityNumber,


                    UserType = request.UserType,

                    OfficeId = request.OfficeId,

                    RegionId = request.RegionId

                };
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    _responseDTO.Result = null;
                    _responseDTO.StatusEnum = StatusEnum.Exception;
                    _responseDTO.Message = _stringLocalizer["anErrorOccurredPleaseContactSystemAdministrator"];

                    return _responseDTO;
                }

                user.Code = SendSMS.GenerateCode();
                var meass = _stringLocalizer["Mobileverificationcode"];
             
                await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                var foundedUser = await _userManager.Users.Where(x => x.UserType == request.UserType &&
                                            x.IdentityNumber == request.IdentityNumber).FirstOrDefaultAsync();
                if (foundedUser != null)
                {
                    await _userManager.DeleteAsync(foundedUser);
                }

                _responseDTO.Result = null;
                _responseDTO.StatusEnum = StatusEnum.Exception;
                //  _responseDTO.Message = _localizationProvider.Localize("anErrorOccurredPleaseContactSystemAdministrator", _auditService.UserLanguage);
                _responseDTO.Message = _stringLocalizer["anErrorOccurredPleaseContactSystemAdministrator"];

            }
            
            _responseDTO.Message = "AddedSuccessfully";

            _responseDTO.StatusEnum = StatusEnum.SavedSuccessfully;

            return _responseDTO;


        }

    }
}

