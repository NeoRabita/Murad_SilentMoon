using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, string queueName, CancellationToken ct = default);
    }
}
