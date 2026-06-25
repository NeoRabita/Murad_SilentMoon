using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System;

public class CustomMessagingResponseFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var requestParam = context.ApiDescription.ParameterDescriptions
            .FirstOrDefault(p => p.Type != null && IsMessagingInterface(p.Type));

        if (requestParam != null)
        {
            var messagingInterface = requestParam.Type.GetInterfaces()
                .First(i => i.IsGenericType && (i.GetGenericTypeDefinition().Name.Contains("ICommand") ||
                                               i.GetGenericTypeDefinition().Name.Contains("IQuery")));

            var responseType = messagingInterface.GetGenericArguments()[0];

            var schema = context.SchemaGenerator.GenerateSchema(responseType, context.SchemaRepository);

            if (!operation.Responses.ContainsKey("200"))
            {
                operation.Responses.Add("200", new OpenApiResponse { Description = "Success" });
            }

            operation.Responses["200"].Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = schema
                }
            };
        }
    }

    private bool IsMessagingInterface(Type type)
    {
        return type.GetInterfaces().Any(i => i.IsGenericType &&
            (i.GetGenericTypeDefinition().Name.StartsWith("ICommand") ||
             i.GetGenericTypeDefinition().Name.StartsWith("IQuery")));
    }
}