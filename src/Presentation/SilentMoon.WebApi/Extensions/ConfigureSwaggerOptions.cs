using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace SilentMoon.WebApi.Extensions
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly IWebHostEnvironment _environment;
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IWebHostEnvironment environment)
        {
            _provider = provider;
            _environment = environment;
        }
        public void Configure(SwaggerGenOptions options)
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var envBadge = _environment.EnvironmentName switch
            {
                "Development" => $"🟢{_environment.EnvironmentName}",
                "Staging" => $"🟡{_environment.EnvironmentName}",
                _ => $"{_environment.EnvironmentName}"
            };
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = $"{assemblyName} - v{description.ApiVersion} {envBadge}",
                    Version = description.ApiVersion.ToString(),
                    Description = "This Api will be responsible for overall data distribution and authorization. Created by Nijat <3",
                    Contact = new OpenApiContact
                    {
                        Name = "nijat.net",
                        Email = "contact@nijat.net",
                    }
                });
            }
        }
    }
}
