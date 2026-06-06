using CoreBanking.Worker.Consumers;

namespace CoreBanking.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly TransactionConsumer _transactionConsumer;

    public Worker(ILogger<Worker> logger, TransactionConsumer transactionConsumer)
    {
        _logger = logger;
        _transactionConsumer = transactionConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.UtcNow);
            }
            await Task.Delay(1000, stoppingToken);
            await _transactionConsumer.StartAsync(stoppingToken);
        }
    }
}
