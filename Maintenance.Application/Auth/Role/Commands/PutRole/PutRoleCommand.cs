using Maintenance.Application.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Application.Auth.Role.Commands.UpdateRole
{
    public class PutRoleCommand : IRequest<ResponseDTO>
    {
        public long Id { get; set; }    

        [Required(ErrorMessageResourceName = "RoleName_Required")]
        public string Name { get; set; }
        //public List<long> Permissions { set; get; }

    }
}
