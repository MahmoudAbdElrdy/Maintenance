using AutoMapper;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Mapper;

namespace Maintenance.Application.Features.RequestsComplanit.Dto
{
    public class ComplanitDto 
    {
        public string Code { get; set; }
        public string CategoryComplanitName { set; get; }
        public string? CategoryComplanitLogo { get; set; }
        public long? CategoryComplanitId { get; set; }
        public long? RequestComplanitId { get; set; }
        public string? Description { get; set; }
        public List<CheckListComplanitDto> CheckListComplanit { get; set; }
        public string[] AttachmentsComplanit { get; set; }
        public int? ComplanitStatus { get; set; }
        public string location { get; set; } 
        public string SerialNumber { get; set; }
        public DateTime? CreatedOn { get; set; }
        public UserDto? UserDto { get; set; } 
    }
    public class UserDto 
    {
        public long? UserId { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IdentityNumber { get; set; }

    }
    public class CheckListComplanitDto:IHaveCustomMapping
    {
        public long? CheckListComplanitId { get; set; } 
        public string? Name { get; set; }
      
        public string? Description { get; set; }

        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<CheckListComplanit, CheckListComplanitDto>()
                .ForMember(x=>x.CheckListComplanitId,opt=>opt.MapFrom(x=>x.Id))
                .ReverseMap();
        }
    }
}
