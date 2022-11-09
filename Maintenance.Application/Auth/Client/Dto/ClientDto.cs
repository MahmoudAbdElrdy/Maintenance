using AuthDomain.Entities.Auth;
using AutoMapper;

using Maintenance.Domain.Enums;
using Maintenance.Domain.Mapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Application.Auth.Client.Dto
{
    public class ClientDto : IHaveCustomMapping
    {
        public string? FullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public UserType? UserType { get; set; }
        public string? IdentityNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public State State { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }

        public long? RoomId { set; get; }
        public string? LoginCode { get; set; }
        public string? SmsCode { get; set; }
        public bool? SmsVerify { get; set; }
        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<User, ClientDto>();

        }
    }
}
