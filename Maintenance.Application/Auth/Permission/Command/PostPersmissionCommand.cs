using Maintenance.Application.Helper;
using MediatR;

namespace Maintenance.Application.Auth.Permission.Command
{
    public class PostPersmissionCommand : IRequest<ResponseDTO>
    {
        public long RoleId { set; get; }
        public List<PostPermissionDTO> Permissions { set; get; }

    }
}
