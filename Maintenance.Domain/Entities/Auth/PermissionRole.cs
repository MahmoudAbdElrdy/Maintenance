using AuthDomain.Entities.Auth;
using Maintenance.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Domain.Entities.Auth
{
    public class PermissionRole
    {
        [ForeignKey("Role")]
        public long RoleId { set; get; }
        [ForeignKey("Permission")]
        public long PermissionId { set; get; }
        public bool IsChecked { set; get; }
        public State State { get; set; }
        public virtual Role Role { set; get; }
        public virtual Permission Permission { set; get; }
        public PermissionAuthorize PermissionAuthorize { get; set; }

    }
}
