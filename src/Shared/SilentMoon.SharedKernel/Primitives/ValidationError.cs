namespace SilentMoon.SharedKernel.Primitives
{
    public sealed record ValidationError : Error
    {
        public ValidationError(Error[] errors, Dictionary<string, Dictionary<string, object>>? placeholders = null)
            : base(
                "Validation.General",
                "One or more validation errors occurred",
                ErrorType.Validation)
        {
            Errors = errors;
            Placeholders = placeholders ?? new();
        }

        public Error[] Errors { get; }
        public Dictionary<string, Dictionary<string, object>> Placeholders { get; }

        public static ValidationError FromResults(IEnumerable<Result> results) =>
            new(results.Where(r => r.IsFailure).Select(r => r.Error).ToArray());
    }
}
