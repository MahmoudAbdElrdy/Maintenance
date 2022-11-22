using AuthDomain.Entities.Auth;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Domain.Entities.Complanits
{
    public class RequestComplanit : IBaseEntity, IAuditable, ISoftDelete
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public State State { get; set; }
        [ForeignKey("Creator")]
        public long? CreatedBy { set; get; }
        public virtual User Creator { get; set; }
        public string? Description { get; set; }
        //public long? OfficeId { get; set; }
       // public long? RegionId { get; set; } 
        public string SerialNumber { get; set; } 
        public string Code { get; set; }  
        public string CodeSms { get; set; }   
        public virtual ICollection<CheckListRequest> CheckListRequests { get; set; } = new List<CheckListRequest>();
        public virtual ICollection<AttachmentComplanit> AttachmentsComplanit { get; set; } = new List<AttachmentComplanit>();
        public virtual ICollection<ComplanitHistory> ComplanitHistory { get; set; } = new List<ComplanitHistory>();
    }
}
