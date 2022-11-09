
using Maintenance.Application.Auth.Login;

namespace Maintenance.Application.Features.Account.Commands.Login
{
    public class AuthorizedUserDTO
    {
        public AuthorizedUserDTO()
        {

        }


        public UserDto User { get; set; }
        public string Token { get; set; }
        public string Code { get; set; }
    }
}
