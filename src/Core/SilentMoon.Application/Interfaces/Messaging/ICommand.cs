namespace Application.Abstractions.Messaging;

public interface IBaseCommand { }
public interface IBaseNonTransactionalCommand : IBaseCommand { }

public interface ICommand<out TResponse> : IBaseCommand { }
public interface ICommand : IBaseCommand { }

public interface INonTransactionalCommand<out TResponse> : IBaseNonTransactionalCommand { }
public interface INonTransactionalCommand : IBaseNonTransactionalCommand { }
