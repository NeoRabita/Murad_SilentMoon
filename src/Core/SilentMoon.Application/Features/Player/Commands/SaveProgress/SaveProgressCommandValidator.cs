using FluentValidation;

namespace SilentMoon.Application.Features.Player.Commands.SaveProgress
{
    public class SaveProgressCommandValidator : AbstractValidator<SaveProgressCommand>
    {
        public SaveProgressCommandValidator()
        {
            RuleFor(x => x.TrackId)
                .GreaterThan(0);

            RuleFor(x => x.PositionSeconds)
                .GreaterThanOrEqualTo(0);
        }
    }
}
