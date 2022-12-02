using AuthDomain.Entities.Auth;
using AutoMapper;
using Maintenance.Domain.Enums;
using Maintenance.Domain.Mapper;

namespace Maintenance.Application.Features.Users.Commands.LoginWeb
{
    public class UserDto : IHaveCustomMapping
    {
        public long? UserId { get; set; }
        public UserType? UserType { get; set; }
        public string? IdentityNumber { get; set; }
        public string? Code { get; set; }
        public string PhoneNumber { get; set; }
        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<User, UserDto>()
                .ForMember(c=>c.UserId,opt=>opt.MapFrom(x=>x.Id))
               
                .ReverseMap();
          
        }
    }
}
