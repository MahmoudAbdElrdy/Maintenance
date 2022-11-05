using Maintenance.Domain.Enums;

namespace Maintenance.Domain.Entities.Auth
{
    public class Permission
    {
        public long Id { set; get; }
        public string Name { get; set; }
        public string ControllerName { set; get; }
        public string ActionName { set; get; }
        public bool? IsChecked { set; get; }
        public State State { get; set; }
        public virtual ICollection<PermissionRole> PermissionRoles { set; get; }

    }
}
