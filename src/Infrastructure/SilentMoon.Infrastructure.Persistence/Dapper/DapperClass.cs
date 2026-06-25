using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using SilentMoon.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace SilentMoon.Infrastructure.Persistence.Dapper
{
    internal class DapperClass : IDapper
    {
        private readonly AppDbContext _context;
        public DapperClass(AppDbContext context)
        {
            _context = context;
        }

        public IDbTransaction? CurrentTransaction =>
             _context.Database.CurrentTransaction?.GetDbTransaction();

        public IDbConnection GetConnection() => _context.Database.GetDbConnection();

        public async Task<T?> GetAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            var cnn = GetConnection();
            return await cnn.QueryFirstOrDefaultAsync<T>(sql, parameters,
                transaction: CurrentTransaction,
                commandType: commandType);
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            var cnn = GetConnection();
            return await cnn.QueryAsync<T>(sql, parameters,
                transaction: CurrentTransaction,
                commandType: commandType);
        }

        public async Task<int> ExecuteAsync(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            var cnn = GetConnection();
            return await cnn.ExecuteAsync(sql, parameters,
                transaction: CurrentTransaction,
                commandType: commandType);
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            var cnn = GetConnection();

            return await cnn.ExecuteScalarAsync<T>(
                sql,
                parameters,
                transaction: CurrentTransaction,
                commandType: commandType);
        }
    }
}