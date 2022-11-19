
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthDomain.Entities.Auth
{
    public class User : IdentityUser<long>, IAuditable, ISoftDelete
    {
        public string? FullName { get; set; }
        public UserType? UserType { get; set; }
        public string? IdentityNumber { get; set; } 
        public HashSet<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
        public DateTime CreatedOn { get ; set ; }
        public DateTime? UpdatedOn { get ; set ; }
        public State State { get ; set ; }
        public long? CreatedBy { get ; set ; }
        public long? UpdatedBy { get ; set ; }
        public long RoomId { set; get; }
        public string? Code { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? Token { get; set; }
        public string? WebToken { get; set; }
        public virtual ICollection<RequestComplanit> RequestComplanits { get; set; } = new List<RequestComplanit>();

    }

}