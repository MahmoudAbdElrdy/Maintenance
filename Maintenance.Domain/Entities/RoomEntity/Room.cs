using AuthDomain.Entities.Auth;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maintenance.Domain.Entities.RoomEntity
{
    public class Room : IBaseEntity, IAuditable, ISoftDelete
    {
        public long Id { get; set; }

        [ForeignKey("Creator")]
        public long? CreatedBy { set; get; }
        public User Creator { get; set; }
        public DateTime CreatedOn { set; get; }
        public long? UpdatedBy { set; get; }
        public DateTime? UpdatedOn { set; get; }
        public State State { set; get; }
        public int RoomNumber { get; set; }
        public int LimittedNumber { get; set; }
        public string? QRImage { get; set; }
        [ForeignKey("Carvan")]
        public long CarvanId { set; get; }
        public CarvanEntity.Carvan Carvan { get; set; }
       

    }
}