using System;
using System.Collections.Generic;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthDomain.Entities.Auth
{
    //[Table("AspNetUsers")]
    public class User : IdentityUser<long>, IAuditable, ISoftDelete
    {
        public string FirstName { get; set; }
        public string WebToken { get; set; }
        public string UserLang { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public UserType UserType { get; set; }
      
        public HashSet<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
        public DateTime CreatedOn { get ; set ; }
        public DateTime? UpdatedOn { get ; set ; }
        public State State { get ; set ; }
        public long? CreatedBy { get ; set ; }
        public long? UpdatedBy { get ; set ; }
    }

}