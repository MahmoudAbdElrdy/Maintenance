using AuthDomain.Entities.Auth;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Domain.Entities.Complanits
{
    public class AttachmentComplanit : IBaseEntity, IAuditable, ISoftDelete
    {
       // [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public long Id { set; get; }
        public DateTime CreatedOn { set; get; }
        public long? UpdatedBy { set; get; }
        public DateTime? UpdatedOn { set; get; }

        [Required, MaxLength(2000)]
        public string? Path { set; get; }
        public string? Name { set; get; }
        public State State { get; set; }
        [ForeignKey("Creator")]
        public long? CreatedBy { set; get; }
        public virtual User Creator { get; set; }

        public long? RequestComplanitId { set; get; }

        [ForeignKey("RequestComplanitId")]

        public virtual RequestComplanit RequestComplanit { get; set; }


    }
}
