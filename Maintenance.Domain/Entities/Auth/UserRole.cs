using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuthDomain.Entities.Auth {
  public class UserRole : IdentityUserRole<long>, ISoftDelete
    {
        [Required, ForeignKey("UserId")]
        public User User { get; set; }

        [Required, ForeignKey("RoleId")]
        public Role Role { get; set; }

        public State State { get; set; }
    }

}
