using System.Threading;
using System.Threading.Tasks;

namespace Application.Abstractions.Messaging;
public interface ICommandHandler<in TCommand>
    where TCommand : IBaseCommand
{
    Task<Result> Handle(TCommand command, CancellationToken ct);
}

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : IBaseCommand
{
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken ct);
}
