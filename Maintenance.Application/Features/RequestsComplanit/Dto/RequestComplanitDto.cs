using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Application.Features.Categories.Dto;
using Maintenance.Application.Helpers.Paginations;
using Maintenance.Domain.Entities.Complanits;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Mapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public List<string>? RegionId { get; set; }
        public List<string>? OfficeId { get; set; }
        public ComplanitStatus? ComplanitStatus { get; set; }
    }
}
