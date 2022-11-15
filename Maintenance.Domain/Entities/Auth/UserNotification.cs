using AuthDomain.Entities.Auth;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Domain.Entities.Auth
{
    public class UserNotification : IBaseEntity, IAuditable, ISoftDelete
    {
        public long Id { get; set; }
        public State State { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
       
       
        public bool Read { get; set; } = false;
        [ForeignKey("NotficiationId")]
        public long NotficiationId { get; set; }
        public Notification Notification { get; set; }
        [ForeignKey("UserId")]
        public long UserId { get; set; }
        public User User { get; set; }
        public DateTime? ReadDate { get; set; }
    }
}
