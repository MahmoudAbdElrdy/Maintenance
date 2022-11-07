using Maintenance.Application.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Application.Auth.UserRole.Commands.AddRole
{
    public class PostRoleCommand : IRequest<ResponseDTO>
    {

        [Required(ErrorMessageResourceName = "RoleName_Required")]
        public string? RoleName { get; set; }
        //public List<long> Permissions { set; get; }
    }
}
