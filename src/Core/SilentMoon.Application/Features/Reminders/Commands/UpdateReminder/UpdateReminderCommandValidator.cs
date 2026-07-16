using FluentValidation;

namespace SilentMoon.Application.Features.Reminders.Commands.UpdateReminder
{
    public class UpdateReminderCommandValidator : AbstractValidator<UpdateReminderCommand>
    {
        public UpdateReminderCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
