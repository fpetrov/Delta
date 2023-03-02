using Microsoft.Extensions.Logging;
using Showcast.TelegramBot.Services.Telegram.Abstract;
using Telegram.Bot;

namespace Showcast.TelegramBot.Services.Telegram;

// Compose Receiver and UpdateHandler implementation
public class ReceiverService : ReceiverServiceBase<UpdateHandler>
{
    public ReceiverService(
        ITelegramBotClient botClient,
        UpdateHandler updateHandler,
        ILogger<ReceiverServiceBase<UpdateHandler>> logger)
        : base(botClient, updateHandler, logger)
    {
    }
}
