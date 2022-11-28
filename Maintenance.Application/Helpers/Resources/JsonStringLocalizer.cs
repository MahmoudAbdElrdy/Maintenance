using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace dotnet_6_json_localization
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly IDistributedCache _cache;
        private readonly JsonSerializer _serializer = new();
        private readonly IWebHostEnvironment _env; 
        public JsonStringLocalizer(IDistributedCache cache, IWebHostEnvironment env)
        {
            _cache = cache;
            _env = env;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }
        public LocalizedString this[string name,string lang]
        {
            get
            {
                var value = GetStringLang(name,lang);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }
        public LocalizedString this[string name, params object[] arguments]
        {
            //get
            //{
            //    var actualValue = this[name];
            //    return !actualValue.ResourceNotFound
            //        ? new LocalizedString(name, string.Format(actualValue.Value, arguments), false)
            //        : actualValue;
            //}
            get
            {
                var value = GetStringLang(name, (string)arguments[0]);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var filePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";
            using var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var sReader = new StreamReader(str);
            using var reader = new JsonTextReader(sReader);
            while (reader.Read())
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    continue;
                string? key = reader.Value as string;
                reader.Read();
                var value = _serializer.Deserialize<string>(reader);
                yield return new LocalizedString(key, value, false);
            }
        }
        //https://zetcode.com/csharp/path/
        private string? GetString(string key)
        {
            string? relativeFilePath = null;
            var fullPath1 = Path.Combine("/Resources",Thread.CurrentThread.CurrentCulture.Name+".json");
            if (_env.IsDevelopment())
            {
                relativeFilePath = $"D:\\Ahmed_Alaa\\Maintenance\\Maintenance\\Maintenance.Application\\Helpers\\Resources\\{Thread.CurrentThread.CurrentCulture.Name}.json";
            }
            else
            {
                relativeFilePath = $"Resources/{Thread.CurrentThread.CurrentCulture.Name}.json";

            }
         
           
            if (File.Exists(relativeFilePath))
            {
                var cacheKey = $"locale_{Thread.CurrentThread.CurrentCulture.Name}_{key}";
                var cacheValue = _cache.GetString(cacheKey);
                if (!string.IsNullOrEmpty(cacheValue))
                {
                    return cacheValue;
                }

                var result = GetValueFromJSON(key, Path.GetFullPath(relativeFilePath));

                if (!string.IsNullOrEmpty(result))
                {
                    _cache.SetString(cacheKey, result);

                }
                return result;
            }
            return default;
        }
        private string? GetValueFromJSON(string propertyName, string filePath)
        {
            if (propertyName == null)
            {
                return default;
            }
            if (filePath == null)
            {
                return default;
            }
            using var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var sReader = new StreamReader(str);
            using var reader = new JsonTextReader(sReader);
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName && reader.Value as string == propertyName)
                {
                    reader.Read();
                    return _serializer.Deserialize<string>(reader);
                }
            }
            return default;
        }
        private string? GetStringLang(string key,string lang)
        {
            string? relativeFilePath = null;
            var fullPath1 = Path.Combine("/Resources", lang + ".json");
            if (_env.IsDevelopment())
            {
                relativeFilePath = $"D:\\Ahmed_Alaa\\Maintenance\\Maintenance\\Maintenance.Application\\Helpers\\Resources\\{lang}.json";
            }
            else
            {
                relativeFilePath = $"Resources/{lang}.json";

            }


            if (File.Exists(relativeFilePath))
            {
                var cacheKey = $"locale_{lang}_{key}";
                var cacheValue = _cache.GetString(cacheKey);
                if (!string.IsNullOrEmpty(cacheValue))
                {
                    return cacheValue;
                }

                var result = GetValueFromJSON(key, Path.GetFullPath(relativeFilePath));

                if (!string.IsNullOrEmpty(result))
                {
                    _cache.SetString(cacheKey, result);

                }
                return result;
            }
            return default;
        }
    }
}
