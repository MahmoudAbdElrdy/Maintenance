using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Helper;
using Maintenance.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Maintenance.Application.Auth.Client.Command
{
    public class ClientRegisterCommand :  IRequest<ResponseDTO>
    {
        public string FullName { get; set; }
        public UserType UserType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IdentityNumber { get; set; }
        public string[] Roles { get; set; }
        public long RoomId { set; get; }
        public string Password { get; set; }
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
            try
            {
                var user = new User()
                {
                    UserName = request.FullName.Split(" ")[0],

                    Email = request.FullName + "@gmail.com",
                  
                    FullName = request.FullName,

                    PhoneNumber = request.PhoneNumber,

                    NormalizedEmail = request.FullName + "@GMAIL.com",

                    NormalizedUserName = request.FullName.Split(" ")[0].ToUpper(),

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
                 await _userManager.AddToRolesAsync(user, request.Roles);
                _responseDTO.Result = null;
                _responseDTO.Message = "userAddedSuccessfully";
                _responseDTO.StatusEnum = StatusEnum.SavedSuccessfully;
            }
            catch (Exception ex)
            {

                _responseDTO.Result = null;
                _responseDTO.StatusEnum = StatusEnum.Exception;
                _responseDTO.Message = "anErrorOccurredPleaseContactSystemAdministrator";
            }
            

            return _responseDTO;


        }
    }
}

