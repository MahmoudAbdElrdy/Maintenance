namespace Maintenance.Application.Auth.Role.Queries.GetAllRolesWithoutPagination
{
    public class GetAllUserRolesDTO
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string ProfileImage { get; set; }
        public string UserMail { get; set; }
        public string UserTitle { get; set; }
        public DateTime? UpdatedOn { set; get; }
        public long CreatedBy { set; get; }
        public List<long> Permissions { set; get; }
    }
}
