using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Paginations;
using MediatR;

namespace Maintenance.Application.Auth.Role.Queries.GetUsersRole
{
    public class GetUsersRoleQuery : IRequest<ResponseDTO>
    {
        public GetUsersRoleQuery()
        {
            PaginatedInputModel = new PaginatedInputModel();
        }
        public long RoleId { get; set; }
        public PaginatedInputModel PaginatedInputModel { get; set; }
    }
}
