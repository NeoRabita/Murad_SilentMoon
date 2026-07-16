using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SilentMoon.Application;
using SilentMoon.Infrastructure.Persistence;
using SilentMoon.WebApi.Extensions;

namespace SilentMoon.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.DisableDefaultApiValidation();
            services.AddControllers();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddHttpContextAccessor();
            services.AddApplicationLayer();
            services.AddPersistenceRegistration(Configuration);
            services.AddPersistenceApiServices(Configuration);
            services.AddJwtAuthentication(Configuration);
            services.AddSwaggerExtension();
            services.AddLocalization();
            services.AddServiceExtension();
            services.EnableApiVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
              app.UseDeveloperExceptionPage();
            }

            app.UseLocalization();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseErrorHandling();
            app.UseSwaggerExtension(env, provider);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
