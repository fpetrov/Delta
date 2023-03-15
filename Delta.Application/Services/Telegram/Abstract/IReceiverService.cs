namespace Delta.Application.Services.Telegram.Abstract;

public interface IReceiverService
{
    public Task ReceiveAsync(CancellationToken stoppingToken);
}