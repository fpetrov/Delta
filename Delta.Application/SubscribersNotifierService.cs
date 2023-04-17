using Delta.Application.Entities;
using Delta.Application.Services.Olimpiad;
using Delta.Application.Services.User;
using Delta.Dnevnik;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Delta.Application;

/// <summary>
/// Service for automated newsletter for bot users.
/// </summary>
public class SubscribersNotifierService : BackgroundService
{
    private readonly ILogger<SubscribersNotifierService> _logger;
    private readonly IUserService _userService;
    private readonly IOlimpiadService _olimpiadService;
    private readonly ITelegramBotClient _botClient;
    private readonly PeriodicTimer _periodicTimer;

    public SubscribersNotifierService(ILogger<SubscribersNotifierService> logger,
        IDnevnikPlainConnection dnevnikConnection, IOlimpiadService olimpiadService, IUserService userService,
        ITelegramBotClient botClient)
    {
        _logger = logger;
        _olimpiadService = olimpiadService;
        _userService = userService;
        _botClient = botClient;
        _periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(10));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (await _periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            _logger.LogInformation("Tick");

            var users = _userService.FindAll(cancellationToken);

            await foreach (var user in users)
            {
                var incomingMathOlimpiads = _olimpiadService.FindIncoming(user.OlimpiadType, cancellationToken);
                
                await foreach (var olimpiad in incomingMathOlimpiads)
                {
                    var text =
                        $"Олимпиада ***{olimpiad.Name}*** пройдет {olimpiad.Expires.ToShortDateString()}. Сайт олимпиады: {olimpiad.Url}";

                    await _botClient.SendTextMessageAsync(
                        chatId: user.TelegramId,
                        text: text,
                        ParseMode.Markdown,
                        cancellationToken: cancellationToken);
                }


                var incomingNotifications = _userService.FindIncomingNotifications(user, cancellationToken);
                
                await foreach (var notification in incomingNotifications)
                {
                    var text =
                        $"Напоминание ***{notification.Name}*** на {notification.Expires.ToShortDateString()}. \n `{notification.Descriptions}`";

                    await _botClient.SendTextMessageAsync(
                        chatId: user.TelegramId,
                        text: text,
                        ParseMode.Markdown,
                        cancellationToken: cancellationToken);
                }

                await _userService.RemoveNotifications(user, incomingNotifications, cancellationToken);
            }
        }
    }
}