using FluentValidation;

namespace SilentMoon.Application.Features.Reminders.Commands.CreateReminder
{
    public class CreateReminderCommandValidator : AbstractValidator<CreateReminderCommand>
    {
        public CreateReminderCommandValidator()
        {
            RuleFor(x => x.Time)
                .NotEqual(default(System.TimeSpan));

            RuleFor(x => x.Days)
                .NotEqual(SilentMoon.Domain.Enums.DaysOfWeekFlags.None);
        }
    }
}
