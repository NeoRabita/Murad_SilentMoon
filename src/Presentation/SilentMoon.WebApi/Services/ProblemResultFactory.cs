using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Localization;
using System.Linq;
using SilentMoon.SharedKernel.Primitives;
using SilentMoon.SharedKernel.Resources;

namespace SilentMoon.WebApi.Services;

public class ProblemResultFactory(IStringLocalizer<Messages> localizer) : IProblemResultFactory
{
    public IResult CreateProblem(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create a problem result for a successful operation.");
        }

        var extensions = new Dictionary<string, object?>
            {
                { "errorCode", result.Error.Code }
            };

        if (result.Error is ValidationError validationError)
        {
            var translatedErrors = validationError.Errors.Select(e =>
            {
                var localizedString = localizer[e.Code];
                string template = localizedString.ResourceNotFound ? e.Description : localizedString.Value;

                string finalMessage = template;

                foreach (var pSet in validationError.Placeholders.Values)
                {
                    if (pSet.TryGetValue("PropertyName", out var propName) && e.Description.Contains(propName.ToString()))
                    {
                        foreach (var placeholder in pSet)
                        {
                            finalMessage = finalMessage.Replace($"{{{placeholder.Key}}}", placeholder.Value?.ToString());
                        }
                    }
                }

                return new { code = e.Code, description = finalMessage };
            }).ToList();

            extensions.Add("errors", translatedErrors);
        }

        return Results.Problem(
            title: GetTitle(result.Error),
            detail: GetDetail(result.Error),
            type: GetType(result.Error.Type),
            statusCode: GetStatusCode(result.Error.Type),
            extensions: extensions);

        string GetTitle(Error error) => localizer["ErrorType." + Enum.GetName(error.Type)];

        string GetDetail(Error error) {

            var localizedString = localizer[error.Code];
            string errorCodeDetail = localizedString.ResourceNotFound ? localizer["ErrorType." + Enum.GetName(error.Type) + "Detail"] : localizedString.Value;

            return errorCodeDetail;
        }
        string GetType(ErrorType errorType) =>
           errorType switch
           {
               ErrorType.Validation or ErrorType.InvalidRequest or ErrorType.Problem => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
               ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
               ErrorType.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
               ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
               ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
               ErrorType.InvalidState => "https://tools.ietf.org/html/rfc4918#section-11.2",
               ErrorType.LimitExceeded => "https://tools.ietf.org/html/rfc6585#section-4",
               ErrorType.Unavailable => "https://tools.ietf.org/html/rfc7231#section-6.6.4",
               _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
           };

        int GetStatusCode(ErrorType errorType) =>
           errorType switch
           {
               ErrorType.Validation or ErrorType.InvalidRequest or ErrorType.Problem => StatusCodes.Status400BadRequest,
               ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
               ErrorType.Forbidden => StatusCodes.Status403Forbidden,
               ErrorType.NotFound => StatusCodes.Status404NotFound,
               ErrorType.Conflict => StatusCodes.Status409Conflict,
               ErrorType.InvalidState => StatusCodes.Status422UnprocessableEntity,
               ErrorType.LimitExceeded => StatusCodes.Status429TooManyRequests,
               ErrorType.ExternalProvider => StatusCodes.Status502BadGateway,
               ErrorType.Unavailable => StatusCodes.Status503ServiceUnavailable,
               ErrorType.Unexpected or ErrorType.Failure => StatusCodes.Status500InternalServerError,
               _ => StatusCodes.Status500InternalServerError
           };
    }
}
