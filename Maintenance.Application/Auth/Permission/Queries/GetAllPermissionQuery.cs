using Maintenance.Application.Helper;
using MediatR;

namespace Maintenance.Application.Auth.Permission.Queries
{
    public class GetAllPermissionQuery : IRequest<ResponseDTO>
    {
        public long RoleId { get; set; }
    }
}