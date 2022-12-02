using MediatR;
using System.ComponentModel.DataAnnotations;
using Maintenance.Application.Helper;

namespace Maintenance.Application.Features.Users.Commands.LoginWeb
{
    public class LoginWebQuery : IRequest<ResponseDTO>
    {

        [Required(ErrorMessage = "NationalId Required")]
        public string NationalId { get; set; }

        [Required(ErrorMessage = "Password Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Code Required")]
        public string Code { get; set; }
    }
}
