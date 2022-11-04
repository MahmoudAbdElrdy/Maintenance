using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;


namespace AuthDomain.Entities.Auth {
  public class Role : IdentityRole {
    public Role(string name) : base(name) {
      Name = name;
    }
    public HashSet<UserRole> UserRoles { get; set; }
    public string Permissions { get; set; }
  }

}
