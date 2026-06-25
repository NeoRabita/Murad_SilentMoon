using System.Collections.Generic;
using System.Threading.Tasks;
using SilentMoon.Application.DTOs.ViewModels;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Enums;

namespace SilentMoon.Application.Interfaces.Repositories
{
    public interface IPomodoroRepository : IGenericRepository<Pomodoro>
    {
        public Task<IEnumerable<PomodoroViewModel>> GetUserPomodoros(string userId);
        public List<PomodoroColors> GetPomodoroColors();
        public Task<PomodoroDetailsViewModel> GetPomodoroDetails(string userId, int pomodoroId);
        public Task<string> CreatePomodoroLog(int pomodoroId);

    }
}