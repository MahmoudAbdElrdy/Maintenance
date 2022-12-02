using Maintenance.Domain.Enums;

namespace Maintenance.Application.Features.Users.Queries.GetUserByIdQuery
{
    public class UsersDTO
    {
        public long  Id { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IdentityNumber { get; set; }
        public long? OfficeId { get; set; }
        public long? RegionId { get; set; }
    }
}
