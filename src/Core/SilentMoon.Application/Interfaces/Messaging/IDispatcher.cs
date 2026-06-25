using Application.Abstractions.Messaging;
using System.Threading.Tasks;
using System.Threading;

public interface IDispatcher
{
    // COMMAND (NO RESULT)
    Task<Result> Send(IBaseCommand command, CancellationToken ct = default);

    // COMMAND (RESULT<T>)
    Task<Result<TResult>> Send<TResult>(ICommand<TResult> command, CancellationToken ct = default);

    Task<Result<TResult>> Send<TResult>(INonTransactionalCommand<TResult> command, CancellationToken ct = default);

    // QUERY (RESULT<T>)
    Task<Result<TResult>> Send<TResult>(IQuery<TResult> query, CancellationToken ct = default);
}