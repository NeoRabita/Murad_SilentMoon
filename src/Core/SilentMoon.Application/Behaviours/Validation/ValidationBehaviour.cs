using FluentValidation;
using FluentValidation.Results;
using SilentMoon.Application.Interfaces.Messaging;
using SilentMoon.SharedKernel.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Behaviours.Validation
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<Task<TResponse>> next)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count == 0)
                return await next();

            var error = CreateValidationError(failures);

            return CreateFailure(error);
        }

        private static TResponse CreateFailure(Error error)
        {
            // CASE 1: TResponse = Result
            if (typeof(TResponse) == typeof(Result))
            {
                return (TResponse)(object)Result.Failure(error);
            }

            // CASE 2: TResponse = Result<T>
            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var innerType = typeof(TResponse).GetGenericArguments()[0];

                var failureMethod = typeof(Result)
                    .GetMethods()
                    .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethod);

                var genericFailure = failureMethod
                    .MakeGenericMethod(innerType)
                    .Invoke(null, new object[] { error });

                return (TResponse)genericFailure!;
            }

            throw new InvalidOperationException(
                $"TResponse must be Result or Result<T>, but got {typeof(TResponse)}");
        }

        private static ValidationError CreateValidationError(List<ValidationFailure> validationFailures)
        {
            var errors = validationFailures
                .Select(f => Error.Validation(f.ErrorCode, f.ErrorMessage))
                .ToArray();

            var placeholders = validationFailures
                .ToDictionary(
                    f => $"{f.PropertyName}_{f.ErrorCode}",
                    f => f.FormattedMessagePlaceholderValues
                );

            return new ValidationError(errors, placeholders);
        }
    }
}