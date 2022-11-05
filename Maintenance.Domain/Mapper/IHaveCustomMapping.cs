using AutoMapper;

namespace Maintenance.Domain.Mapper
{
    public interface IHaveCustomMapping
    {
        void CreateMappings(Profile configuration);
    }
}