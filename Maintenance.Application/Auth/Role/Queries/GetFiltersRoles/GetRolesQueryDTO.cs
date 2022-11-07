using System;
using System.Collections.Generic;

namespace Maintenance.Application.Auth.Role.Queries.GetFiltersRoles
{
    public class GetRolesQueryDTO
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public string Name_AR { set; get; }
        public string RoleDescription { get; set; }
        public DateTime? UpdatedOn { set; get; }
        public long CreatedBy { set; get; }
        public string CreatedByNameAR { set; get; }
        public string CreatedByNameEN { set; get; }
        public string Code { set; get; }
        public List<long> Permissions { set; get; }
    }
}
