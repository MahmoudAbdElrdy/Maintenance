using AutoMapper;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Interfaces;
using Maintenance.Domain.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string OfficeName { get; set; } 
        public string RegionName { get; set; } 
        public string CarvanNumber { get; set; } 
        public string RoomNumber { get; set; } 
        public string SerialNumber { get; set; }
        public DateTime? CreatedOn { get; set; }
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
