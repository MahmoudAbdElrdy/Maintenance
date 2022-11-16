using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;

namespace dotnet_6_json_localization
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IDistributedCache _cache;
        private readonly IWebHostEnvironment _env;

        public JsonStringLocalizerFactory(IDistributedCache cache, IWebHostEnvironment env)
        {
            _cache = cache;
            _env = env;
        }

        public IStringLocalizer Create(Type resourceSource) =>
            new JsonStringLocalizer(_cache, _env);

        public IStringLocalizer Create(string baseName, string location) =>
           new JsonStringLocalizer(_cache, _env);
    }
}
