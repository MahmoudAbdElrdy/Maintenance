namespace Maintenance.Application.Auth.Role.Queries.GetAllRoles
{
    public class GetRolesQueryDTO
    {
        public long Id { get; set; }
        public string Name { set; get; }
        public string RoleDescription { get; set; }
        public DateTime? UpdatedOn { set; get; }
        public long CreatedBy { set; get; }
        public string Code { set; get; }
        public List<long> Permissions { set; get; }
    }
}
