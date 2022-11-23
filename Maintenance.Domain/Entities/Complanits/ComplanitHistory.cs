using AuthDomain.Entities.Auth;
using Maintenance.Domain.Entities.Auth;
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
    public class ComplanitHistory : IBaseEntity, IAuditable, ISoftDelete
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? Description { get; set; }
        public bool? IsApprove { get; set; }
        public State State { get; set; }
        public ComplanitStatus? ComplanitStatus { get; set; }

        [ForeignKey("Creator")]
        public long? CreatedBy { set; get; }
        public virtual User Creator { get; set; }
       
        public long? RequestComplanitId { set; get; }
       
        [ForeignKey("RequestComplanitId")]
      
        public virtual RequestComplanit RequestComplanit { get; set; }

        public virtual ICollection<AttachmentComplanitHistory> AttachmentComplanitHistory { get; set; } = new List<AttachmentComplanitHistory>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();


    }

}
