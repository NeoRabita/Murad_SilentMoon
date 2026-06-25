using Application.Abstractions.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Messaging
{
    public class Dispatcher : IDispatcher
    {
        private readonly IServiceProvider _provider;

        public Dispatcher(IServiceProvider provider)
        {
            _provider = provider;
        }

        // ============================
        //  COMMAND (NO RESULT)
        // ============================
        public async Task<Result> Send(IBaseCommand command, CancellationToken ct = default)
        {
            var commandType = command.GetType();

            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            dynamic handler = _provider.GetRequiredService(handlerType);

            var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(commandType, typeof(Result));
            var pipelines = _provider.GetServices(pipelineType).Cast<dynamic>().ToList();

            Func<Task<Result>> handlerDelegate = () => handler.Handle((dynamic)command, ct);

            var pipelineChain = pipelines.Reverse<dynamic>()
                .Aggregate(handlerDelegate, (next, pipeline) =>
                    () => pipeline.Handle((dynamic)command, ct, next));

            return await pipelineChain();
        }

        // ============================
        //  QUERY (RESULT<T>)
        // ============================
        public async Task<Result<TResult>> Send<TResult>(IQuery<TResult> query, CancellationToken ct = default)
        {
            var queryType = query.GetType();

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));
            dynamic handler = _provider.GetRequiredService(handlerType);

            var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(queryType, typeof(Result<TResult>));
            var pipelines = _provider.GetServices(pipelineType).Cast<dynamic>().ToList();

            Func<Task<Result<TResult>>> handlerDelegate = () => handler.Handle((dynamic)query, ct);

            var pipelineChain = pipelines.Reverse<dynamic>()
                .Aggregate(handlerDelegate, (next, pipeline) =>
                    () => pipeline.Handle((dynamic)query, ct, next));

            return await pipelineChain();
        }

        // ============================
        //  COMMAND<TResult> (Transactional & Non-Transactional)
        // ============================

        public Task<Result<TResult>> Send<TResult>(ICommand<TResult> command, CancellationToken ct = default)
            => ExecuteInternal<TResult>(command, ct);

        public Task<Result<TResult>> Send<TResult>(INonTransactionalCommand<TResult> command, CancellationToken ct = default)
            => ExecuteInternal<TResult>(command, ct);

        private async Task<Result<TResult>> ExecuteInternal<TResult>(IBaseCommand command, CancellationToken ct)
        {
            var commandType = command.GetType();

            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));
            dynamic handler = _provider.GetRequiredService(handlerType);

            var pipelineType = typeof(IPipelineBehavior<,>).MakeGenericType(commandType, typeof(Result<TResult>));
            var pipelines = _provider.GetServices(pipelineType).Cast<dynamic>().ToList();

            Func<Task<Result<TResult>>> handlerDelegate = () => handler.Handle((dynamic)command, ct);

            var pipelineChain = pipelines.Reverse<dynamic>()
                .Aggregate(handlerDelegate, (next, pipeline) =>
                    () => pipeline.Handle((dynamic)command, ct, next));

            return await pipelineChain();
        }
    }

    // ============================
    //  DEPENDENCY INJECTION
    // ============================
    public static class ServiceCollectionExtensions
    {
        public static void AddCqrsHandlers(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract);

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces().Where(i => i.IsGenericType);

                foreach (var iface in interfaces)
                {
                    var def = iface.GetGenericTypeDefinition();

                    if (def == typeof(ICommandHandler<>) ||
                        def == typeof(ICommandHandler<,>) ||
                        def == typeof(IQueryHandler<,>) ||
                        def == typeof(IPipelineBehavior<,>))
                    {
                        if (type.IsGenericTypeDefinition)
                        {
                            services.AddScoped(def, type);
                        }
                        else
                        {
                            services.AddScoped(iface, type);
                        }
                    }
                }
            }
        }
    }
}