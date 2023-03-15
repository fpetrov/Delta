using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Delta.Application;

/// <summary>
/// Service for automated newsletter for bot users.
/// </summary>
public class SubscribersNotifierService : BackgroundService
{
    private readonly ILogger<SubscribersNotifierService> _logger;
    private readonly PeriodicTimer _periodicTimer;

    public SubscribersNotifierService(ILogger<SubscribersNotifierService> logger)
    {
        _logger = logger;
        _periodicTimer = new PeriodicTimer(TimeSpan.FromDays(1));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (await _periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            _logger.LogInformation("1 Second");
        }
    }
}