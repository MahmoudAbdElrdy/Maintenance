using Maintenance.Application.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Application.Features.Account.Queries.CheckLoginWeb
{
    public class CheckLoginWebQuery : IRequest<ResponseDTO>
    {
        [Required(ErrorMessage = "NationalId Required")]
        public string IdentityNumber { get; set; } 

        [Required(ErrorMessage = "Password Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Code Required")]
        public string Code { get; set; }
    }
}
