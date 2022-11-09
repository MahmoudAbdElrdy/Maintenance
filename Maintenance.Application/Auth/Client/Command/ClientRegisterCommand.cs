using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Auth.Login;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.SendSms;
using Maintenance.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Maintenance.Application.Auth.Client.Command
{
    public class ClientRegisterCommand :  IRequest<ResponseDTO>
    {
        public string? FullName { get; set; }
        public UserType UserType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IdentityNumber { get; set; }
        public string[]? Roles { get; set; }
        public long RoomId { set; get; }
        public string? Password { get; set; }
    }
    class Handler : IRequestHandler<ClientRegisterCommand, ResponseDTO>
    {

        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ResponseDTO _responseDTO;
        public Handler(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _responseDTO = new ResponseDTO();
        }
        public async Task<ResponseDTO> Handle(ClientRegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new User();
            try
            {
                 user = new User()
                {
                    UserName = request.IdentityNumber,

                    Email = request.PhoneNumber,
                  
                    FullName = request.FullName,

                    PhoneNumber = request.PhoneNumber,

                    NormalizedEmail = request.PhoneNumber,

                    NormalizedUserName = request.IdentityNumber,

                    CreatedOn = DateTime.Now,

                    State = State.NotDeleted,

                    IdentityNumber = request.IdentityNumber,

                    RoomId = request.RoomId,
                    
                };
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    _responseDTO.Result = null;
                    _responseDTO.StatusEnum = StatusEnum.Exception;
                    _responseDTO.Message = "anErrorOccurredPleaseContactSystemAdministrator";
                }
                 if (request.Roles.Length > 0)
                {
                    await _userManager.AddToRolesAsync(user, request.Roles);
                  
                }
               
                user.Code = SendSMS.GenerateCode();
                var res = await SendSMS.SendMessageUnifonic("رمز التحقق من الجوال : " + user.Code, user.PhoneNumber);
                if (res == -1)
                {

                    if (await _userManager.FindByNameAsync(user.UserName) != null)
                    {
                        await _userManager.DeleteAsync(user);
                    }
                    _responseDTO.Message = "حدث خطا فى ارسال الكود";
                    _responseDTO.StatusEnum = StatusEnum.Failed;
                    return _responseDTO;
                }
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
                _responseDTO.Message = "anErrorOccurredPleaseContactSystemAdministrator";
            }
            _responseDTO.Result = _responseDTO.Result = _mapper.Map<UserDto>(user);
            _responseDTO.Message = "userAddedSuccessfully";
            _responseDTO.StatusEnum = StatusEnum.SavedSuccessfully;

            return _responseDTO;


        }
       
    }
}

