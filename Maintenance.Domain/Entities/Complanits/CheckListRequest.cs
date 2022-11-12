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

        [ForeignKey("RequestComplanit")]
        public long? RequestComplanitId { set; get; }
        public virtual RequestComplanit RequestComplanit { get; set; }

        [ForeignKey("CheckListComplanit")]
        public long? CheckListComplanitId { set; get; } 
        public virtual CheckListComplanit CheckListComplanit { get; set; }
    }
    
}
