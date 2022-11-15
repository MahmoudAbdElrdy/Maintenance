using Maintenance.Application.Helpers.Resources;
using Maintenance.Application.Interfaces;
using Maintenance.Domain.Interfaces;
using System.Globalization;
using System.Resources;
namespace Maintenance.Application.Helpers
{
    public class LocalizationProvider : ILocalizationProvider
    {
        private readonly IResourceSourceManager _resourceSourceManager;
        private readonly IAuditService _auditService;

        public LocalizationProvider(IResourceSourceManager resourceSourceManager, IAuditService auditService)
        {
            _resourceSourceManager = resourceSourceManager;
            _auditService = auditService;

        }

        public string Localize(string str, string cultureName)
        {
            var result = TryLocalize(str, cultureName);
            if (!string.IsNullOrWhiteSpace(result))
                return result;

            return str;
        }
        public string Localize(string str)
        {
            string cultureName = _auditService.UserLanguage;
            var result = TryLocalize(str, cultureName);
            if (!string.IsNullOrWhiteSpace(result))
                return result;

            return str;
        }

        public string Localize(string str, string cultureName, params object[] args)
        {
            return string.Format(Localize(str, cultureName), args);
        }

        private string TryLocalize(string str, string cultureName)
        {
            var ci = new CultureInfo(cultureName);
            var resourceManager = new ResourceManager(typeof(SharedResource));
            var result = resourceManager.GetString(str, ci);
            return result;
        }
    }
    public class LocalizationManager : ILocalizationManager
    {
        public LocalizationManager(ILocalizationProvider localizationProvider, IAuditService auditService)
        {
            LocalizationProvider = localizationProvider;
            AuditService = auditService;
        }

        public ILocalizationProvider LocalizationProvider { get; }
        public IAuditService AuditService { get; }
    }
    public class ResourceSourceManager : IResourceSourceManager, ISingletonDependency
    {
        protected IList<Type> RegisteredSourceTypes { get; }

        public ResourceSourceManager()
        {
            RegisteredSourceTypes = new List<Type>();
        }

        public IReadOnlyList<Type> GetRegisteredSourceTypes()
        {
            return RegisteredSourceTypes.ToList();
        }

        public void RegisterSourceType(Type type)
        {
            RegisteredSourceTypes.Add(type);
        }
    }

}
