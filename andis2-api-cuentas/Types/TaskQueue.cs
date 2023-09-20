using MediatR;
using System.Threading.Channels;

namespace andis2_api_cuentas.Types;

public interface ITaskQueue
{
    Task<INotification> DequeueTaskAsync(CancellationToken cancellationToken);
    Task QueueTaskAsync(INotification task, CancellationToken cancellationToken = default);
}

public class TaskQueue : ITaskQueue
{
    private readonly Channel<INotification> _queue = Channel.CreateUnbounded<INotification>();

    public async Task QueueTaskAsync(INotification task, CancellationToken cancellationToken = default)
    {
        await _queue.Writer.WriteAsync(task);
    }

    public async Task<INotification> DequeueTaskAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}