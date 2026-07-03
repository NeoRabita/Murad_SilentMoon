using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SilentMoon.Application.Interfaces.Messaging;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Application.Services;

namespace SilentMoon.Application
{
    public static class ServiceExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddScoped<IDispatcher, Dispatcher>();
            services.AddCqrsHandlers(Assembly.GetExecutingAssembly());
            services.AddScoped<IOtpSender, OtpSender>();
        }
    }
}