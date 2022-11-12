using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Features.Categories.Dto;
using Maintenance.Domain.Entities.Reports;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Mapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Application.Features.RequestsReport.Dto 
{
    public class RequestReportDto:IHaveCustomMapping
    {
        public long Id { get; set; }
        public string? Description { get; set; }
        public long []? CheckListsRequest { get; set; }
        public long []? AttachmentsReport { get; set; } 
        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<RequestReport,RequestReportDto>().ReverseMap();
        }
    }
}
