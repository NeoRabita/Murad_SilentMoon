using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SilentMoon.Application.DTOs.Reminder;
using SilentMoon.Application.Interfaces.Messaging;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Entities.SilentMoon.Domain.Entities;
using SilentMoon.Application.Interfaces.Services;
using SilentMoon.Domain.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Messaging
{
    public class ReminderSchedulerService : BackgroundService
    {
        private const string QueueName = "reminder.email";

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ReminderSchedulerService> _logger;

        public ReminderSchedulerService(IServiceScopeFactory scopeFactory,ILogger<ReminderSchedulerService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(ct))
            {
                try
                {
                    await CheckDueRemindersAsync(ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Reminder check error");
                }
            }
        }

        private async Task CheckDueRemindersAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();

            var uow = scope.ServiceProvider.GetRequiredService<IUow>();
            var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
            var dateTimeService = scope.ServiceProvider.GetRequiredService<IDateTimeService>();

            var now = dateTimeService.localTime;
            var todayFlag = now.DayOfWeek.ToDaysOfWeekFlags();

            var reminderRepo = uow.GetRepository<Reminder>();
            var userRepo = uow.GetRepository<ApplicationUser>();

            var reminders = await reminderRepo.GetAllAsync(ct);

            var dueReminders = reminders
                .Where(x => x.IsActive)
                .Where(x => (x.Days & todayFlag) == todayFlag)
                .Where(x => x.Time.Hours == now.Hour && x.Time.Minutes == now.Minute)
                .ToList();

            if (dueReminders.Count == 0)
            {
                return;
            }

            var userIds = dueReminders
                .Select(x => x.ApplicationUserId)
                .Distinct();

            var users = await userRepo.GetByIdsAsync(userIds, ct);
            var usersById = users.ToDictionary(x => x.Id);

            foreach (var reminder in dueReminders)
            {
                if (!usersById.TryGetValue(reminder.ApplicationUserId, out var user))
                {
                    continue;
                }

                var message = new ReminderEmailMessage
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    Time = reminder.Time
                };

                await publisher.PublishAsync(message, QueueName, ct);
            }
        }
    }
}
