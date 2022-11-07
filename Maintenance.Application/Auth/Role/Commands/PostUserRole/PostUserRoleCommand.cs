using Maintenance.Application.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Application.Auth.Role.Commands.PostUserRole
{
    public class PostUserRoleCommand : IRequest<ResponseDTO>
    {

        public long RoleId { get; set; }
        public long UserId { get; set; }
        //public List<long> Permissions { set; get; }
    }
}
