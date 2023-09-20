using andis2_api_cuentas.Types;
using MediatR;

namespace andis2_api_cuentas.Services;

public sealed class TaskProcessingService : BackgroundService
{
    private readonly ITaskQueue _taskQueue;
    private readonly ILogger<TaskProcessingService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TaskProcessingService(ITaskQueue taskQueue, ILogger<TaskProcessingService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _taskQueue = taskQueue;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Service} is starting.", nameof(TaskProcessingService));
        return ProcessTaskQueueAsync(cancellationToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Waiting for task");
                var task = await _taskQueue.DequeueTaskAsync(cancellationToken);

                using var scope = _serviceScopeFactory.CreateScope();
                var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

                _logger.LogInformation("Executing task");
                await publisher.Publish(task, cancellationToken);
                _logger.LogInformation("Task executed");
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing task");
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Service} is stopping.", nameof(TaskProcessingService));
        await base.StopAsync(cancellationToken);
    }
}