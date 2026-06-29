using Microsoft.EntityFrameworkCore.Storage;
using SilentMoon.Application.Interfaces.Repositories;
using SilentMoon.Infrastructure.Persistence.Contexts;
using SilentMoon.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class Uow : IUow
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    private readonly Dictionary<Type, object> _repositories = new();


    public Uow(AppDbContext context)
    {
        _context = context;
    }


    public IGenericRepository<T> GetRepository<T>()
        where T : class
    {
        if (_repositories.ContainsKey(typeof(T)))
        {
            return (IGenericRepository<T>)_repositories[typeof(T)];
        }


        var repository = new GenericRepository<T>(_context);

        _repositories.Add(typeof(T), repository);

        return repository;
    }


    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }


    public async Task BeginTransactionAsync()
    {
        if (_transaction != null) return;

        _transaction = await _context.Database
            .BeginTransactionAsync();
    }


    public async Task CommitAsync()
    {
        try
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }


    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();

            _transaction = null;
        }
    }


    public void Dispose()
    {
        _context.Dispose();
        _transaction?.Dispose();
    }
}