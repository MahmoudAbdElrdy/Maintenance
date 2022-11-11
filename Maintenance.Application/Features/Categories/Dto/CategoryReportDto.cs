using AutoMapper;
using Maintenance.Domain.Entities.Reports;
using Maintenance.Domain.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Application.Features.Categories.Dto
{
    public class CategoryReportDto : IHaveCustomMapping
    {
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }
        public string? Description { get; set; }
        public long Id { get; set; }
        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<CategoryReport, CategoryReportDto>().ReverseMap();
        }
    }
}
