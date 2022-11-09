using MediatR;
using Maintenance.Application.Helper;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Application.Features.Account.Commands.Login
{
    public class LoginQuery : IRequest<ResponseDTO>
    {

        [Required(ErrorMessage = "Identity Number Required")]
        public string IdentityNumber { get; set; } 

        [Required(ErrorMessage = "Password Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

       // [Required(ErrorMessage = "Code Required")]
        public string Code { get; set; } 
    }
}
