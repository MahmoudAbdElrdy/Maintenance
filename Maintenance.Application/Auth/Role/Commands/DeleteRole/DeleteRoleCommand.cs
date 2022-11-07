using MediatR;
using Maintenance.Application.Helper;

namespace Maintenance.Application.Auth.Role.Commands.DeleteRole
{
    public class DeleteRoleCommand : IRequest<ResponseDTO>
    {
        public long id { get; set; }
    }
}
