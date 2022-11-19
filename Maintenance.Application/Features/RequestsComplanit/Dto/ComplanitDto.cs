using AutoMapper;
using Maintenance.Domain.Entities.Complanits;
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
        
        public string? Description { get; set; }
        public string CategoryComplanitName{ set; get; } 
        public List<CheckListComplanitDto> CheckListComplanit { get; set; }
        public long? RequestComplanitId { get; set; }

        public string[] AttachmentsComplanit { get; set; }
        public long? CategoryComplanitId { get; set; }
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
