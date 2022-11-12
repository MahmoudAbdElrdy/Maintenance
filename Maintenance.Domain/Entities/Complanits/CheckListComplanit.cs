using AuthDomain.Entities.Auth;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Domain.Entities.Complanits
{
    public class CheckListComplanit : IBaseEntity, IAuditable, ISoftDelete 
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public State State { get; set; }
        [ForeignKey("Creator")]
        public long? CreatedBy { set; get; }
        public virtual User Creator { get; set; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        [ForeignKey("CategoryComplanit")]
        public long? CategoryComplanitId { set; get; }
        public virtual CategoryComplanit? CategoryComplanit { get; set; }
        public virtual ICollection<CheckListRequest> CheckListRequests { get; set; } = new List<CheckListRequest>();

    }
}
