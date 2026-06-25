using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Dapper
{
    public interface IDapper
    {
        IDbConnection GetConnection();
        IDbTransaction? CurrentTransaction { get; }

        Task<T?> GetAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text);

        Task<IEnumerable<T>> GetAllAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text);

        Task<int> ExecuteAsync(string sql, object? parameters = null, CommandType commandType = CommandType.Text);

        Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text);
    }
}