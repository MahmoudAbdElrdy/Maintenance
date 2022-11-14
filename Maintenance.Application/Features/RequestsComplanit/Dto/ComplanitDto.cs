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
    public class ComplanitDto : IHaveCustomMapping
    {
        
        public string? Description { get; set; }
        public string CategoryComplanitName{ set; get; } 
        public string[] CheckListsRequest { get; set; }

        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<CheckListRequest, ComplanitDto>().
                ForMember(x => x.CategoryComplanitName,
                dto => dto.MapFrom(x => x.CheckListComplanit.CategoryComplanit.NameAr)) .
                 ForMember(x => x.Description,
                dto => dto.MapFrom(x => x.RequestComplanit.Description))
                .ForMember(x => x.CheckListsRequest,
                dto => dto.MapFrom(x => x.CheckListComplanit.CheckListRequests.Select(x=>x.CheckListComplanit.NameAr)));
        }
    }
}
