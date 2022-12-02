
using Maintenance.Application.Auth.Login;

namespace Maintenance.Application.Features.Users.Commands.LoginWeb
{
    public class AuthorizedUserDTO
    {
        public AuthorizedUserDTO()
        {

        }


        public UserDto User { get; set; }
        public string Token { get; set; }
        
    }
}
