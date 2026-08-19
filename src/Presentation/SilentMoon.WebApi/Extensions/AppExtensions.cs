using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SilentMoon.Infrastructure.Persistence.Services;
using SilentMoon.SharedKernel.Resources;
using SilentMoon.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;

namespace SilentMoon.WebApi.Extensions
{
    public static class AppExtensions
    {
        public static void UseSwaggerExtension(this IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment() || env.IsStaging() || env.IsProduction())
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


        public static void UseCurrentUser(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var currentUser = context.RequestServices.GetRequiredService<CurrentUser>();

                    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (int.TryParse(userIdClaim, out var userId))
                    {
                        currentUser.UserId = userId;
                    }

                    currentUser.UserName = context.User.FindFirst(ClaimTypes.Name)?.Value;
                }

                await next();
            });
        }

        private static readonly HashSet<string> PublicControllers = new(StringComparer.OrdinalIgnoreCase)
        {
            "Account",
            "Topics",
            "Tracks",
            "Categories"
        };

        public static void UseAuthEnforcement(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var controllerName = context.GetEndpoint()?.Metadata
                    .GetMetadata<ControllerActionDescriptor>()?
                    .ControllerName;

                if (controllerName == null)
                {
                    // Not an MVC controller action (Swagger, static files, etc.) — nothing to protect here.
                    await next();

                    return;
                }

                var isPublic = PublicControllers.Contains(controllerName);

                if (!isPublic && context.User.Identity?.IsAuthenticated != true)
                {
                    var localizer = context.RequestServices.GetRequiredService<IStringLocalizer<Messages>>();

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var envelope = new ErrorEnvelope
                    {
                        Error = new ErrorDetail
                        {
                            Code = "Error.Unauthorized",
                            Message = localizer["ErrorType.UnauthorizedDetail"].Value,
                            RequestId = context.TraceIdentifier
                        }
                    };

                    await context.Response.WriteAsJsonAsync(envelope);

                    return;
                }

                await next();
            });
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

                    context.Response.StatusCode = statusCode;
                    context.Response.ContentType = "application/json";

                    var envelope = new ErrorEnvelope
                    {
                        Error = new ErrorDetail
                        {
                            Code = $"Error.{errorType}",
                            Message = localizer["ErrorType." + Enum.GetName(errorType) + "Detail"].Value,
                            RequestId = context.TraceIdentifier
                        }
                    };

                    await context.Response.WriteAsJsonAsync(envelope);
                }
            }));
        }
    }
}
