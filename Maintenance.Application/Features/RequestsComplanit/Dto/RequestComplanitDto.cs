using Maintenance.Domain.Enums;

namespace Maintenance.Application.Features.RequestsComplanit.Dto
{
    public class RequestComplanitDto
    {
       
        public string? Description { get; set; }
        public long []? CheckListsRequest { get; set; }
        public string []? AttachmentsComplanit { get; set; } 
        public string? SerialNumber { get; set; }


    }
    public class FilterComplanitDto
    {
       public int PageNumber { get; set; } 
        public int PageSize { get; set; }
        public List<long>? CategoryId { get; set; }
        public List<long>? RegionId { get; set; }
        public List<long>? OfficeId { get; set; }
        public ComplanitStatus? ComplanitStatus { get; set; }
    }
}
