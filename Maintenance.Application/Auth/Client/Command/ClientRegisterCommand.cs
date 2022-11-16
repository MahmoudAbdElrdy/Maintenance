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

namespace Maintenance.Application.Auth.Client.Command
{
    public class ClientRegisterCommand :  IRequest<ResponseDTO>
    {
        public string? FullName { get; set; }
        public UserType UserType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IdentityNumber { get; set; }
        public string[]? Roles { get; set; }
        public long RoomNumber { set; get; }
        public string? Password { get; set; }
    }
    class Handler : IRequestHandler<ClientRegisterCommand, ResponseDTO>
    {

        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ResponseDTO _responseDTO;
        private readonly IRoom _room;
      
        private readonly IAuditService _auditService;
        private readonly IStringLocalizer<ClientRegisterCommand> _stringLocalizer;
        public Handler(UserManager<User> userManager, IMapper mapper, IRoom room, IStringLocalizer<ClientRegisterCommand> stringLocalizer, IAuditService auditService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _responseDTO = new ResponseDTO();
            _room = room;
            _auditService = auditService;
            _stringLocalizer = stringLocalizer;
        }
        public async Task<ResponseDTO> Handle(ClientRegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new User();
            try
            {
                long room =0;
                try
                {
                   room = await _room.GetRoomId(request.RoomNumber);
                  
                 if (room == 0)
                
                    {
                        _responseDTO.Result = request.RoomNumber;

                        _responseDTO.StatusEnum = StatusEnum.Failed;

                        _responseDTO.Message = _stringLocalizer["RoomNotFound"].ToString();

                        return _responseDTO;
                    }
                   
                }
                catch (ApiException ex)
                {
                    _responseDTO.Result = null;
                   
                    _responseDTO.StatusEnum = StatusEnum.Failed;
                  
                    _responseDTO.Message = _stringLocalizer["anErrorOccurredPleaseContactSystemAdministrator"];
                    return _responseDTO;
                }
                var checkExsit= await _userManager.Users.Where(x => x.IdentityNumber == request.IdentityNumber).FirstOrDefaultAsync();
               
                if (checkExsit != null)
                {
                    _responseDTO.Result = null;
                    _responseDTO.StatusEnum = StatusEnum.Failed;
                  
                   // _responseDTO.Message = _localizationProvider.Localize("NationalNumberFoundBefore", _auditService.UserLanguage);
                    _responseDTO.Message = _stringLocalizer["NationalNumberFoundBefore"];
                    return _responseDTO;
                }
                var phoneExsit = await _userManager.Users.Where(x => x.PhoneNumber == request.PhoneNumber).FirstOrDefaultAsync();
               
                if (phoneExsit != null)
                {
                    _responseDTO.Result = null;
                    _responseDTO.StatusEnum = StatusEnum.Failed;
                    _responseDTO.Message = _stringLocalizer["PhoneNumberFoundBefore"];
                  //  _responseDTO.Message = _localizationProvider.Localize("PhoneNumberFoundBefore", _auditService.UserLanguage);

                    return _responseDTO;
                }

                 user = new User()
                {
                    UserName = request.IdentityNumber,

                    Email = request.PhoneNumber+"@Gamil.com",
                  
                    FullName = request.FullName,

                    PhoneNumber = request.PhoneNumber,

                    NormalizedEmail = request.PhoneNumber+ "@Gamil.com",

                    NormalizedUserName = request.IdentityNumber,

                    CreatedOn = DateTime.Now,

                    State = State.NotDeleted,

                    IdentityNumber = request.IdentityNumber,

                    RoomId = room,

                    UserType = request.UserType,
                    
                };
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    _responseDTO.Result = null;
                    _responseDTO.StatusEnum = StatusEnum.Exception;
                    _responseDTO.Message = _stringLocalizer["anErrorOccurredPleaseContactSystemAdministrator"];
                 //   _responseDTO.Message = _localizationProvider.Localize("anErrorOccurredPleaseContactSystemAdministrator", _auditService.UserLanguage);

                    return _responseDTO;
                }
                 if (request.Roles.Count() > 0)
                {
                    await _userManager.AddToRolesAsync(user, request.Roles);
                  
                }
               
                user.Code = SendSMS.GenerateCode();
                //  var meass= _localizationProvider.Localize("Mobileverificationcode", _auditService.UserLanguage);
               var meass = _stringLocalizer["Mobileverificationcode"];
                //var res = SendSMS.SendMessageUnifonic(meass +" : " + user.Code, user.PhoneNumber);
                //if (res == -1)
                //{

                //    if (await _userManager.FindByNameAsync(user.UserName) != null)
                //    {
                //        await _userManager.DeleteAsync(user);
                //    }

                //    _responseDTO.Message = _localizationProvider.Localize("ProplemSendCode", _auditService.UserLanguage);

                //    _responseDTO.StatusEnum = StatusEnum.Failed;
                //    return _responseDTO;
                //}
                await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                if (await _userManager.FindByNameAsync(user.UserName) != null)
                {
                    await _userManager.DeleteAsync(user);
                }
              
                _responseDTO.Result = null;
                _responseDTO.StatusEnum = StatusEnum.Exception;
              //  _responseDTO.Message = _localizationProvider.Localize("anErrorOccurredPleaseContactSystemAdministrator", _auditService.UserLanguage);
                _responseDTO.Message = _stringLocalizer["anErrorOccurredPleaseContactSystemAdministrator"];

            }
            var authorizedUserDto = new AuthorizedUserDTO
            {
                User = _mapper.Map<UserDto>(user),
                Token = null,
            };
            _responseDTO.Result = authorizedUserDto;
         
           // _responseDTO.Message = _localizationProvider.Localize("AddedSuccessfully", _auditService.UserLanguage);
            _responseDTO.Message = _stringLocalizer["AddedSuccessfully"];

            _responseDTO.StatusEnum = StatusEnum.SavedSuccessfully;

            return _responseDTO;


        }
       
    }
}

