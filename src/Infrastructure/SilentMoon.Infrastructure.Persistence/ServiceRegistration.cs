using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Application.Interfaces.Caching;
using SilentMoon.Application.Interfaces.GoogleAuthService;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Messaging;
using SilentMoon.Application.Interfaces.Repositories;
using SilentMoon.Application.Interfaces.Security;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Infrastructure.Persistence.Caching;
using SilentMoon.Infrastructure.Persistence.Contexts;
using SilentMoon.Infrastructure.Persistence.Dapper;
using SilentMoon.Infrastructure.Persistence.Logging;
using SilentMoon.Infrastructure.Persistence.Messaging;
using SilentMoon.Infrastructure.Persistence.Repositories;
using SilentMoon.Infrastructure.Persistence.Services;
using SilentMoon.Infrastructure.Persistence.Settings;
using System;
using System.Linq;
using System.Reflection;

namespace SilentMoon.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            options.UseOracle(configuration["APIAppSettings:ConnectionString"],
            b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));


            services.Configure<RabbitMqSettings>(configuration.GetSection("APIAppSettings:RabbitMqSettings"));
            services.Configure<MinioSettings>(configuration.GetSection("APIAppSettings:MinioSettings"));
            services.AddSingleton<IMinioClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MinioSettings>>().Value;

                return new MinioClient()
                    .WithEndpoint(settings.Endpoint)
                    .WithCredentials(settings.AccessKey, settings.SecretKey)
                    .WithSSL(settings.UseSSL)
                    .Build();
            });
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["APIAppSettings:Redis"];
                options.InstanceName = Assembly.GetEntryAssembly()?.GetName().Name + "_";
            });

            services.AddScoped(typeof(IAppLogger<>), typeof(LoggerManager<>));
            services.AddScoped<ICacheService, RedisCacheService>();
            services.Configure<APIAppSettings>(configuration.GetSection("APIAppSettings"));
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IDateTimeService, DateTimeService>();
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
            services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<IFileStorageService, MinioFileStorageService>();
            services.AddScoped<IOtpSender, OtpSender>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<CurrentUser>();
            services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<CurrentUser>());
            services.AddScoped<IDapper, DapperClass>();
            services.AddScoped<IUow, Uow>();

           
            services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
            services.AddHostedService<OtpEmailConsumer>();
            services.AddHostedService<ReminderSchedulerService>();
            services.AddHostedService<ReminderEmailConsumer>();

            RegisterDapperDomainMappings();
        }
        #region APIRepositories
        public static void AddPersistenceApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        }
        #endregion


        private static void RegisterDapperDomainMappings()
        {
            var domainAssembly = Assembly.Load("SilentMoon.Domain");

            var entityTypes = domainAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Namespace != null && t.Namespace.EndsWith("Entities"));

            foreach (var type in entityTypes)
            {
                var mapperType = typeof(ColumnAttributeTypeMapper<>).MakeGenericType(type);
                var mapper = (SqlMapper.ITypeMap)Activator.CreateInstance(mapperType);

                SqlMapper.SetTypeMap(type, mapper);
            }
        }
    }
}
