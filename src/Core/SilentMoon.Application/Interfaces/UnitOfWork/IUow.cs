using SilentMoon.Application.Interfaces.Repositories;
using System.Threading.Tasks;
using System.Threading;
using System;

public interface IUow : IDisposable
{
    IGenericRepository<T> GetRepository<T>() where T : class;

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task BeginTransactionAsync();

    Task CommitAsync();

    Task RollbackAsync();
}