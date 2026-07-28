using SilentMoon.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        Task<T?> FirstOrDefaultAsync(
       Expression<Func<T, bool>> predicate,
       CancellationToken cancellationToken = default);

        Task<(List<T> Items, int TotalCount)> GetPagedAsync(
            Expression<Func<T, bool>>? predicate,
            Expression<Func<T, object>> orderBy,
            bool ascending,
            int page,
            int limit,
            CancellationToken cancellationToken = default);

        void Update(T entity);

        void Delete(T entity);

        void DeleteRange(IEnumerable<T> entities);
    }
}