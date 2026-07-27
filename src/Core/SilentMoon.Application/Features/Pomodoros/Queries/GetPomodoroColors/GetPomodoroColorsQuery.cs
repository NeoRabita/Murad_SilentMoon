using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Interfaces.Caching;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Domain.Enums;

namespace SilentMoon.Application.Features.Pomodoros.Queries.GetPomodoroColors
{
    public class GetPomodoroColorsQuery : IQuery<List<PomodoroColors>>
    {
    }
    public class GetPomodoroColorsQueryHandler : IQueryHandler<GetPomodoroColorsQuery, List<PomodoroColors>>
    {
        private readonly ICacheService _cacheService;
        private readonly IUow _uow;
        private readonly IAppLogger<GetPomodoroColorsQueryHandler> _logger;
        public GetPomodoroColorsQueryHandler(ICacheService cacheService, IUow uow, IAppLogger<GetPomodoroColorsQueryHandler> logger)
        {
            _cacheService = cacheService;
            _uow = uow;
            _logger = logger;
        }

        private async Task<string> GetTest()
        {
            return await Task.FromResult("salam2222");
        }

        public async Task<Result<List<PomodoroColors>>> Handle(GetPomodoroColorsQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetPomodoroColors strated.");
            var cacheTest = await _cacheService.GetOrAddAsync($"user_c2bb8b5e-3898-4141-9f3c-ebc9787bf7bb", GetTest);

            if (cacheTest == null)
                return Error.NullValue;

            _logger.LogInformation("GetPomodoroColors done.");

            return PomodoroColors.Colors;
        }
    }
}