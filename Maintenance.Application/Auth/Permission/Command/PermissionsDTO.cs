using Maintenance.Domain.Entities.Auth;
using Maintenance.Domain.Enums;

namespace Maintenance.Application.Auth.Permission.Command
{
    public class PostPermissionDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? ControllerName { set; get; }
        public string ActionName { set; get; }
        public State State { get; set; }
        public bool? IsChecked { set; get; }
        public PermissionAuthorize PermissionAuthorize { get; set; }

        public virtual ICollection<PermissionRole>? PermissionRoles { set; get; }
    }
}
