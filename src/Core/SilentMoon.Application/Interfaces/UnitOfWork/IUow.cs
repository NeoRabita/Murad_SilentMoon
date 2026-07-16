using SilentMoon.Application.Interfaces.Repositories;
using SilentMoon.Domain.Common;
using System.Threading.Tasks;
using System.Threading;
using System;

public interface IUow : IDisposable
{
    IGenericRepository<T> GetRepository<T>() where T : BaseEntity;

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task BeginTransactionAsync();

    Task CommitAsync();

    Task RollbackAsync();
}