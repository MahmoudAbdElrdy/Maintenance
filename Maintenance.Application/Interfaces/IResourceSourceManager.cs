using Maintenance.Domain.Interfaces;

namespace Maintenance.Application.Interfaces
{
    public interface IResourceSourceManager : ISingletonDependency
    {
        IReadOnlyList<Type> GetRegisteredSourceTypes();
        void RegisterSourceType(Type type);
    }
    public interface ISingletonDependency
    {

    }
    public interface ILocalizationProvider
    {
        string Localize(string str, string cultureName);
        string Localize(string str, string cultureName, params object[] args);
    }
    public interface ILocalizationManager
    {
        ILocalizationProvider LocalizationProvider { get; }
        IAuditService AuditService { get; } 
    }
}
