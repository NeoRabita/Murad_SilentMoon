using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SilentMoon.SharedKernel.Resources;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SilentMoon.WebApi.Extensions
{
    public static class AppExtensions
    {
        public static void UseSwaggerExtension(this IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment() || env.IsStaging())
            {
                var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            $"{assemblyName}_{description.GroupName.ToUpperInvariant()}");
                    }
                });
            }
        }


        public static void UseLocalization(this IApplicationBuilder app)
        {
            var supportedCultures = new[] { "en-US", "az-AZ", "ru-RU", "en", "az", "ru" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture("en-US")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            localizationOptions.ApplyCurrentCultureToResponseHeaders = true;

            app.UseRequestLocalization(localizationOptions);
        }


        public static void UseErrorHandling(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(handlerApp => handlerApp.Run(async context =>
            {
                var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionFeature?.Error;

                if (exception != null)
                {
                    var localizer = context.RequestServices.GetRequiredService<IStringLocalizer<Messages>>();
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

                    logger.LogError(exception, "System-wide Exception caught: {Message}", exception.Message);

                    var statusCode = StatusCodes.Status500InternalServerError;
                    var errorType = ErrorType.Unexpected;

                    statusCode = exception switch
                    {
                        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                        KeyNotFoundException => StatusCodes.Status404NotFound,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    errorType = statusCode switch
                    {
                        401 => ErrorType.Unauthorized,
                        404 => ErrorType.NotFound,
                        _ => ErrorType.Unexpected
                    };

                    var typeInfo = errorType switch
                    {
                        ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
                        ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                        _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                    };

                    context.Response.StatusCode = statusCode;
                    context.Response.ContentType = "application/problem+json";

                    var problemDetails = new
                    {
                        type = typeInfo,
                        title = localizer[localizer["ErrorType." + Enum.GetName(errorType)]].Value,
                        status = statusCode,
                        detail = localizer["ErrorType." + Enum.GetName(errorType) + "Detail"].Value,
                        errorCode = $"Error.{errorType}"
                    };

                    await context.Response.WriteAsJsonAsync(problemDetails);
                }
            }));
        }
    }
}
