using Maintenance.Domain.Entities.Auth;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthDomain.Entities.Auth {
  public class Role :  IdentityRole<long>, ISoftDelete
    {
    [MaxLength(500)]
    public string? Name { get; set; }
    public string? Code { get; set; }
    public long? CreatedBy { set; get; }
    public DateTime CreatedOn { set; get; }
    public long? UpdatedBy { set; get; }
    public DateTime? UpdatedOn { set; get; }
    public State State { get; set; }
    public virtual ICollection<PermissionRole> PermissionRoles { set; get; }
    public virtual HashSet<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();

    }

}
