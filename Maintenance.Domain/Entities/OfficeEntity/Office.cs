
using AuthDomain.Entities.Auth;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maintenance.Domain.Entities.OfficeEntity
{
    public class Office : IBaseEntity, IAuditable, ISoftDelete
    {
        public long Id { get; set; }
        [ForeignKey("Creator")]
        public long? CreatedBy { set; get; }
        public virtual User Creator { get; set; }
        public DateTime CreatedOn { set; get; }
        public long? UpdatedBy { set; get; }
        public DateTime? UpdatedOn { set; get; }
        public State State { set; get; }
        public string OfficeNameAr { get; set; }
        public string OfficeNameEn { get; set; }
        public virtual ICollection<RegionEntity.Region> Regions { get; set; }
    }
}
