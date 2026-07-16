using Application.Abstractions.Messaging;
using System.Collections.Generic;

namespace SilentMoon.Application.Features.Reminders.Queries.GetReminders
{
    public class GetRemindersQuery : IQuery<List<ReminderResponse>>
    {
    }
}
