using MediatR;
using Maintenance.Application.Helper;
using Maintenance.Application.Helpers.Paginations;

namespace Maintenance.Application.Auth.Role.Queries.GetFiltersRoles
{
    public class GetFiltersRolesQuery : IRequest<ResponseDTO>
    {
        public GetFiltersRolesQuery()
        {
            PaginatedInputModel = new PaginatedInputModel();
        }
        public string?  Name { get; set; }
        public string?  CreatorName { get; set; }
        public DateTime?  StartDate { get; set; }
        public DateTime?  EndDate { get; set; }
        public PaginatedInputModel PaginatedInputModel { get; set; }
    }
}
