using AutoMapper;
using Maintenance.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebHost.Customization;

namespace Maintenance.Application.Helpers
{
    public class LocalizationMappingAction<TSource, TDestination> : MappinActionBase<TSource, TDestination>
    {
        private readonly IAuditService _session;
        protected readonly Dictionary<string, string> Naming;

        public LocalizationMappingAction(IAuditService session)
        {
            _session = session;

            Naming = new Dictionary<string, string>
            {
                {"ar", "En"},
                {"en", "Ar"}
            };
        }

        public override void Process(TSource source, TDestination destination, ResolutionContext context)
        {
            var destinationLocalizedProperties = GetDestinationLicalizationProperties();

            if (!Naming.ContainsKey(_session.UserLanguage))
                throw new Exception("This language is not supported in LocalizationMappingAction<,>");

            var sourceLocalizedProperties = GetSourceLocalizationProperties(Naming[_session.UserLanguage]);

            foreach (var destinationLocalizedProperty in destinationLocalizedProperties)
            {
                var sourceProperty = sourceLocalizedProperties
                    .FirstOrDefault(x => NormalizeLocalizedName(x.Name) == destinationLocalizedProperty.Name);

                if (sourceProperty != null)
                {
                    var value = sourceProperty.GetValue(source);
                    destinationLocalizedProperty.SetValue(destination, value);
                }
            }
        }

        protected List<PropertyInfo> GetDestinationLicalizationProperties()
        {
            var localizationSrouceProperties = GetSourceLocalizationProperties();
            var destinationProperties = typeof(TDestination).GetProperties();

            return destinationProperties.Where(destinationProperty =>
                    localizationSrouceProperties.Any(x => NormalizeLocalizedName(x.Name) == destinationProperty.Name))
                .ToList();
        }

        protected List<PropertyInfo> GetSourceLocalizationProperties()
        {
            var properties = typeof(TSource).GetProperties();
            return properties.Where(IsLocalizationProperty).ToList();
        }

        protected List<PropertyInfo> GetSourceLocalizationProperties(string languagePostfix)
        {
            return GetSourceLocalizationProperties()
                .Where(x => x.Name.EndsWith(languagePostfix, StringComparison.InvariantCulture))
                .ToList();
        }

        protected bool IsLocalizationProperty(PropertyInfo property)
        {
            if (!IsLocalizedConventionPropertyName(property.Name))
                return false;

            var properties = typeof(TSource).GetProperties();
            var expectedNames = GetExpectedNames(property.Name);
            var existingNames = properties.Select(x => x.Name).ToList();

            var joinsList = (from existsing in existingNames
                             join expectedName in expectedNames on existsing equals expectedName
                             select existsing).ToList();

            return joinsList.Count == expectedNames.Length && joinsList.Count > 1;
        }

        protected string NormalizeLocalizedName(string propertyName)
        {
            return IsLocalizedConventionPropertyName(propertyName) ?
                propertyName.Substring(0, propertyName.Length - 2) :
                propertyName;
        }

        private bool IsLocalizedConventionPropertyName(string propertyName)
        {
            foreach (var keyValuePair in Naming)
            {
                if (propertyName.EndsWith(keyValuePair.Value, StringComparison.InvariantCulture))
                    return true;
            }

            return false;
        }

        protected string[] GetExpectedNames(string propertyName)
        {
            var normalizedName = NormalizeLocalizedName(propertyName);
            var result = new List<string>();
            foreach (var keyValuePair in Naming)
            {
                result.Add($"{normalizedName}{keyValuePair.Value}");
            }

            return result.ToArray();
        }
    }
}
