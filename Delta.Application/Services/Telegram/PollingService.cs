using Delta.Application.Services.Telegram.Abstract;
using Microsoft.Extensions.Logging;

namespace Delta.Application.Services.Telegram;

// Compose Polling and ReceiverService implementations
public class PollingService : PollingServiceBase<ReceiverService>
{
    public PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
        : base(serviceProvider, logger)
    {
        
    }
}
