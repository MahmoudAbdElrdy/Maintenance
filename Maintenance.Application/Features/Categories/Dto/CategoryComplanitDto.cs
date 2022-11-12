using AutoMapper;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Application.Features.Categories.Dto
{
    public class CategoryComplanitDto : IHaveCustomMapping
    {
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public long Id { get; set; }
        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<CategoryComplanit, CategoryComplanitDto>().ReverseMap();
        }
    }
}
