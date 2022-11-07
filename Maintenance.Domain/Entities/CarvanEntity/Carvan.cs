using AuthDomain.Entities.Auth;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maintenance.Domain.Entities.CarvanEntity
{
    public class Carvan : IBaseEntity, IAuditable, ISoftDelete
    {
        public long Id { get; set; }
        [ForeignKey("Creator")]
        public long? CreatedBy { set; get; }
        public virtual User Creator { get; set; }
        public DateTime CreatedOn { set; get; }
        public long? UpdatedBy { set; get; }
        public DateTime? UpdatedOn { set; get; }
        public State State { set; get; }
        public int CarvanNumber { get; set; }
        [ForeignKey("Region")]
        public long RegionId { set; get; }
        public RegionEntity.Region Region { get; set; }
        public virtual ICollection<RoomEntity.Room> Rooms { get; set; }

    }
}
