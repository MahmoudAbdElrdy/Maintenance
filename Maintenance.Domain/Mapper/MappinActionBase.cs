using AutoMapper;
using BuildingBlocks.Utilities.Dependency;

namespace WebHost.Customization
{
    public abstract class MappinActionBase<TSource, TDestination> : IMappingAction<TSource, TDestination>, ITransientDependency
    {
        public abstract void Process(TSource source, TDestination destination, ResolutionContext context);       
    }
}