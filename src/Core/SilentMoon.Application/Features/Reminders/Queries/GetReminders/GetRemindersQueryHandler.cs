using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Reminders.Queries.GetReminders
{
    public class GetRemindersQueryHandler : IQueryHandler<GetRemindersQuery, List<ReminderResponse>>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;

        public GetRemindersQueryHandler(
            IUow uow,
            ICurrentUser currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result<List<ReminderResponse>>> Handle(
            GetRemindersQuery query,
            CancellationToken ct)
        {
            var reminderRepo = _uow.GetRepository<Reminder>();

            var reminders = await reminderRepo.GetAllAsync(ct);

            return reminders
                .Where(x => x.ApplicationUserId == _currentUser.UserId)
                .Select(x => new ReminderResponse
                {
                    Id = x.Id,
                    Time = x.Time,
                    Days = x.Days,
                    IsActive = x.IsActive
                })
                .ToList();
        }
    }
}
