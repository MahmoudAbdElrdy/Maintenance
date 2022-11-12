using AutoMapper;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Application.Features.CheckLists.Dto
{
    public class CheckListComplanitDto : IHaveCustomMapping
    {
        public long Id { get; set; }
        public long? CategoryComplanitId { set; get; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }

        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<CheckListComplanit, CheckListComplanitDto>().ReverseMap();
        }
    }
}
