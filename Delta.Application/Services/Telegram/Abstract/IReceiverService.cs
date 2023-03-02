namespace Showcast.TelegramBot.Services.Telegram.Abstract;

public interface IReceiverService
{
    public Task ReceiveAsync(CancellationToken stoppingToken);
}