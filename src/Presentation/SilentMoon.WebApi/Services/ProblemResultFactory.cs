using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Localization;
using System.Linq;
using SilentMoon.SharedKernel.Primitives;
using SilentMoon.SharedKernel.Resources;
using SilentMoon.WebApi.Contracts;

namespace SilentMoon.WebApi.Services;

public class ProblemResultFactory(IStringLocalizer<Messages> localizer) : IProblemResultFactory
{
    public IResult CreateProblem(Result result, string requestId)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot create a problem result for a successful operation.");
        }

        var details = new List<ErrorFieldDetail>();

        if (result.Error is ValidationError validationError)
        {
            details = validationError.Errors.Select(e =>
            {
                var field = validationError.Placeholders.Keys
                    .FirstOrDefault(k => k.EndsWith($"_{e.Code}"));

                field = field != null ? field[..^(e.Code.Length + 1)] : e.Code;

                var localizedString = localizer[e.Code];
                string issue = localizedString.ResourceNotFound ? e.Description : localizedString.Value;

                foreach (var pSet in validationError.Placeholders.Values)
                {
                    if (pSet.TryGetValue("PropertyName", out var propName) && e.Description.Contains(propName.ToString()))
                    {
                        foreach (var placeholder in pSet)
                        {
                            issue = issue.Replace($"{{{placeholder.Key}}}", placeholder.Value?.ToString());
                        }
                    }
                }

                return new ErrorFieldDetail { Field = field, Issue = issue };
            }).ToList();
        }

        var envelope = new ErrorEnvelope
        {
            Error = new ErrorDetail
            {
                Code = result.Error.Code,
                Message = GetMessage(result.Error),
                Details = details,
                RequestId = requestId
            }
        };

        return Results.Json(envelope, statusCode: GetStatusCode(result.Error.Type));

        string GetMessage(Error error)
        {
            var localizedString = localizer[error.Code];
            return localizedString.ResourceNotFound
                ? localizer["ErrorType." + Enum.GetName(error.Type) + "Detail"]
                : localizedString.Value;
        }

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
