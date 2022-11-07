using Maintenance.Domain.Entities.RoomEntity;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthDomain.Entities.Auth
{
    public class User : IdentityUser<long>, IAuditable, ISoftDelete
    {
        public string? FullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public UserType? UserType { get; set; }
        public string? IdentityNumber { get; set; } 
        public HashSet<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
        public DateTime CreatedOn { get ; set ; }
        public DateTime? UpdatedOn { get ; set ; }
        public State State { get ; set ; }
        public long? CreatedBy { get ; set ; }
        public long? UpdatedBy { get ; set ; }
       
        [ForeignKey("Room")]
        public long? RoomId { set; get; }
        public Room? Room { get; set; }
        public string? LoginCode { get; set; }
        public string? SmsCode { get; set; }
        public bool? SmsVerify { get; set; }
    }

}