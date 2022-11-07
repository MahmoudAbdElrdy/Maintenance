using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Application.Auth.Client.Dto
{
    public class ClientDto : IHaveCustomMapping
    {
        public string FirstName { get; set; }
        public string WebToken { get; set; }
        public string UserLang { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public UserType UserType { get; set; }
        public string? MobileNumber { get; set; }
        public string? IdentityNumber { get; set; }
        public string? RoomNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public State State { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<User, ClientDto>();

        }
    }
}
