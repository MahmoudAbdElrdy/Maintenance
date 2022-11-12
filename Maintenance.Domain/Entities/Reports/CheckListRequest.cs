using AuthDomain.Entities.Auth;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Domain.Entities.Reports
{
    public class CheckListRequest : IBaseEntity, IAuditable, ISoftDelete
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public State State { get; set; }
        [ForeignKey("Creator")]
        public long? CreatedBy { set; get; }
        public virtual User Creator { get; set; }

        [ForeignKey("RequestReport")]
        public long? RequestReportId { set; get; }
        public virtual RequestReport RequestReport { get; set; }

        [ForeignKey("CheckListReport")]
        public long? CheckListReportId { set; get; } 
        public virtual CheckListReport CheckListReport { get; set; }
    }
    
}
