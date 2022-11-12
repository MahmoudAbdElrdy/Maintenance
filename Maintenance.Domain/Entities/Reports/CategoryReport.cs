using AuthDomain.Entities.Auth;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maintenance.Domain.Entities.Reports
{
    public class CategoryReport : IBaseEntity, IAuditable, ISoftDelete
    {
        public long Id { get; set; }    
        public DateTime CreatedOn { get  ; set  ; }
        public long? UpdatedBy { get  ; set  ; }
        public DateTime? UpdatedOn { get  ; set  ; }
        public State State { get  ; set  ; }
        [ForeignKey("Creator")]
        public long? CreatedBy { set; get; }
        public virtual User Creator { get; set; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public virtual ICollection<CheckListReport> CheckListsReport { get; set; } = new List<CheckListReport>();
    }
}
