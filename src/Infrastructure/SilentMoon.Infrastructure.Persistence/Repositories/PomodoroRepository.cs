using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SilentMoon.Application.DTOs.ViewModels;
using SilentMoon.Application.Interfaces.Repositories;
using SilentMoon.Domain.Entities;
using SilentMoon.Domain.Enums;
using SilentMoon.Infrastructure.Persistence.Contexts;
using SilentMoon.Infrastructure.Persistence.Dapper;
using Task = System.Threading.Tasks.Task;

namespace SilentMoon.Infrastructure.Persistence.Repositories
{
    public class PomodoroRepository : GenericRepository<Pomodoro>, IPomodoroRepository
    {
        IDapper dapper;
        public PomodoroRepository(IDapper dapper, AppDbContext dbContext) : base(dbContext)
        {
            this.dapper = dapper;
        }

        public async Task<string> CreatePomodoroLog(int pomodoroId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("p_empno", pomodoroId);
            var test = await dapper.ExecuteAsync("PKG_CLIENTS.fire_employee", parameters, commandType: System.Data.CommandType.StoredProcedure);

            return await Task.FromResult("saaa");
        }

        public List<PomodoroColors> GetPomodoroColors()
        {
            return PomodoroColors.Colors;
        }

        public async Task<PomodoroDetailsViewModel> GetPomodoroDetails(string userId, int pomodoroId)
        {
            string sql = @"SELECT Id,Name, PomodoroTime, ShortBreakTime, LongBreakTime, LongBreakInterval, PeriodCount, Color 
                         FROM Pomodoros WHERE UserId = @USER_ID AND IsDeleted = 0 AND Id = @POMODORO_ID";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("USER_ID", userId);
            parameters.Add("POMODORO_ID", pomodoroId);
            return await dapper.GetAsync<PomodoroDetailsViewModel>(sql, parameters);
        }

        public async Task<IEnumerable<PomodoroViewModel>> GetUserPomodoros(string userId)
        {
            string sql = "SELECT Id,Name,PomodoroTime,Color FROM Pomodoros WHERE UserId=@USER_ID AND IsDeleted=0 ORDER by CreateDate DESC";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("USER_ID", userId);
            var UserPomodoros = await dapper.GetAllAsync<PomodoroViewModel>(sql, parameters);
            PomodoroColors defaultcolor = PomodoroColors.Colors.First();
            foreach (var pomodoro in UserPomodoros)
            {
                PomodoroColors color = PomodoroColors.Colors.FirstOrDefault(x => x.Id == pomodoro.Color);
                if (color != null)
                {
                    pomodoro.BgColor = color.BgColor;
                    pomodoro.TxtColor = color.TxtColor;
                }
                else
                {
                    pomodoro.BgColor = defaultcolor.BgColor;
                    pomodoro.TxtColor = defaultcolor.TxtColor;
                }
            }
            return UserPomodoros;
        }
    }
}