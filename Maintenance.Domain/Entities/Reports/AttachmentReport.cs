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

namespace Maintenance.Domain.Entities.Reports
{
    public class AttachmentReport : IBaseEntity, IAuditable, ISoftDelete
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
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

        [ForeignKey("RequestReport")]
        public long RequestReportId { set; get; }
        public virtual RequestReport RequestReport { set; get; }


    }
}
