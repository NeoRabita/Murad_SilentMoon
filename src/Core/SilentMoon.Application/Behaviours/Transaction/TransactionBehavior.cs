using Application.Abstractions.Messaging;
using SilentMoon.Application.Interfaces.Logging;
using SilentMoon.Application.Interfaces.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Behaviours.Transaction
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IBaseCommand
    {
        private readonly IUow _uow;
        private readonly IAppLogger<TransactionBehavior<TRequest, TResponse>> _logger;

        public TransactionBehavior(IUow uow, IAppLogger<TransactionBehavior<TRequest, TResponse>> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<Task<TResponse>> next)
        {
            if (request is IBaseNonTransactionalCommand)
                return await next();

            await _uow.BeginTransactionAsync();

            _logger.LogInformation("Transaction started.");

            try
            {
                var response = await next();

                _logger.LogInformation("Handler successfully processed.");

                await _uow.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("EF core savechanges success.");

                await _uow.CommitAsync();

                _logger.LogInformation("Transaction committed.");

                return response;
            }
            catch (Exception e)
            {
                await _uow.RollbackAsync();
                _logger.LogError(e, "Exception occurred transaction rollbacked");
                throw;
            }
        }
    }
}