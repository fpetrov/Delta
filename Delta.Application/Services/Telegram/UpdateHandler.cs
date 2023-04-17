using Delta.Application.Entities;
using Delta.Application.Messaging.Requests;
using Delta.Application.Repositories.Olimpiad;
using Delta.Application.Services.ChatGPT;
using Delta.Application.Services.Olimpiad;
using Delta.Application.Services.User;
using Delta.Dnevnik.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Delta.Application.Services.Telegram;

// eyJhbGciOiJSUzI1NiJ9.eyJzdWIiOiIyNzI3MzMzIiwiYXVkIjoiMjoxIiwibmJmIjoxNjc3NTE2MzI5LCJtc2giOiIzM2JhZjdmMC03MDA2LTRmZTctOWEwMy0wNWZjMWRkMjg1MzQiLCJhdGgiOiJzdWRpciIsImlzcyI6Imh0dHBzOlwvXC9zY2hvb2wubW9zLnJ1IiwicmxzIjoiezE6WzE4MzoxNjpbXSwzMDo0OltdLDQwOjE6W10sMjA6MjpbXV19IiwiZXhwIjoxNjc4MzgwMzI5LCJpYXQiOjE2Nzc1MTYzMjksImp0aSI6ImUzOTI5MDE1LWZjODUtNGEwNy05MjkwLTE1ZmM4NWZhMDczNyIsInNzbyI6IjhlMTFiYjNhLTgzNDAtNDZlYS04ODAwLTgyZmQyMTU2NGQ5OSJ9.iN2m5vGAGrvt33QYlsOtsyfyszR6vuk4mbFh_P56zrfKdCyiHowj68AwvPbeaIua9h8vIY7VvlxA5zSChJVpvfe-tHH7ikBFg1OJdH-8VU8jHG_QexNn8ieBOVrqwvOqsMKYV-4y7KFW7KAeeE8nCaJu6aQuC2dzp2IOe0IErA4hII2mrt1ML3r2tSUTtf0qsU7JJss57jjMamBL7_icYFHc3HWtK4H5rk-xVIYRA3KgfUZy9co4kdETXb8-wYguaeKJdsTdrCAF-K6MGYbLCrNRP9Fb6xT8-kSSi8k1Q25fE7ZRSj_MFotrk7PNmIZdlIl6ZpoqrXVV8_Umt3LQ3g
public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUserService _userService;
    private readonly IOlimpiadService _olimpiadService;
    private readonly IChatGptService _chatGptService;
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger, IUserService userService, IOlimpiadService olimpiadService, IChatGptService chatGptService)
    {
        _botClient = botClient;
        _logger = logger;
        _userService = userService;
        _olimpiadService = olimpiadService;
        _chatGptService = chatGptService;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        var handler = update switch
        {
            // UpdateType.Unknown:
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            // UpdateType.Poll:
            { Message: { } message }                       => BotOnMessageReceived(message, cancellationToken),
            { EditedMessage: { } message }                 => BotOnMessageReceived(message, cancellationToken),
            _                                              => UnknownUpdateHandlerAsync(update, cancellationToken)
        };

        await handler;
    }

    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive message type: {MessageType}", message.Type);
        if (message.Text is not { } messageText)
            return;

        var action = messageText.Split(' ')[0] switch
        {
            "/login" => SendLogin(_botClient, message, cancellationToken),
            
            "/olimpiad" => CreateOlimpiad(_botClient, message, cancellationToken),
            
            "/schedule" => SendSchedule(_botClient, message, cancellationToken),
            "/homework" => SendHomework(_botClient, message, cancellationToken),
            "/notification" => CreateNotification(_botClient, message, cancellationToken),
            "/notifications" => SendNotifications(_botClient, message, cancellationToken),

            "/stream" => SendStream(_botClient, message, cancellationToken),
            "/start" => Usage(_botClient, message, cancellationToken),
            _ => Explain(_botClient, message, cancellationToken)
        };
        
        var sentMessage = await action;
        _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);

        async Task<Message> Explain(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var response = await _chatGptService.Ask(messageText);
            
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: response,
                ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }

        async Task<Message> SendHomework(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var args = messageText.Split(' ');

            var request = new GetHomeworkRequest(message.Chat.Id, DateTime.Now.AddDays(1));

            if (args.Length > 1)
                request = new GetHomeworkRequest(message.Chat.Id, DateTime.Parse(args[1]));

            var homeworks = _userService.GetHomework(request, cancellationToken);

            var text = $"Вот твое домашнее задание на {request.Date.ToShortDateString()} \n\n";
            
            await foreach (var homework in homeworks)
            {
                text += $"***{homework.Subject}***\n`{homework.Description}`\n\n";
            }

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: text,
                ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }
        
        async Task<Message> SendNotifications(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var notifications = _userService.FindNotifications(message.Chat.Id, cancellationToken);

            var text = "Твои напоминания \n\n";
            
            await foreach (var notification in notifications)
            {
                text += $"***{notification.Name}***\n`{notification.Descriptions}`\n{notification.Expires.ToShortDateString()}\n\n";
            }

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: text,
                ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }

        async Task<Message> CreateOlimpiad(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var args = messageText.Split(' ');
            
            if (!Enum.TryParse<OlimpiadType>(args[3], out var type))
                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Вы ввели неправильный тип олимпиады", cancellationToken: cancellationToken);
            
            var request = new CreateOlimpiadRequest(args[1], args[2], type, DateTime.Parse(args[4]).ToUniversalTime());

            await _olimpiadService.Create(request, cancellationToken);
            
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Олимпиада сохранена.", cancellationToken: cancellationToken);
        }
        
        async Task<Message> CreateNotification(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var args = messageText.Split(' ');

            var request = new AddNotificationRequest(message.Chat.Id, new Notification
            {
                Name = args[1],
                Descriptions = args[2],
                Expires = DateTime.Parse(args[3]).ToUniversalTime()
            });

            await _userService.AddNotification(request, cancellationToken);
            
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Напоминание добавлено.", cancellationToken: cancellationToken);
        }

        async Task<Message> SendLogin(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var args = messageText.Split(' ');
            var dnevnikToken = args[1];
            
            if (!Enum.TryParse<OlimpiadType>(args[2], out var type))
                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Вы ввели неправильный тип направления", cancellationToken: cancellationToken);

            var request = new CreateUserRequest(message.Chat.Username!, message.Chat.Id, dnevnikToken, type);

            await _userService.Create(request, cancellationToken);

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Отлично, ты вошел в аккаунт.", cancellationToken: cancellationToken);
        }
        
        async Task<Message> SendSchedule(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var args = messageText.Split(' ');

            var request = new GetScheduleRequest(message.Chat.Id, DateTime.Now);

            if (args.Length > 1)
                request = new GetScheduleRequest(message.Chat.Id, DateTime.Parse(args[1]));

            var schedule = await _userService.GetSchedule(request, cancellationToken);

            var text = $"Вот твое расписание на {request.Date.ToShortDateString()} \n\n";
            
            var lessonIter = 1;
            foreach (var activity in schedule.Activities)
            {
                if (activity.Type == TypeEnum.Lesson)
                {
                    text += $"{lessonIter}. {activity.Lesson.SubjectName} \n     {activity.BeginTime} - {activity.EndTime} \n\n";
                    lessonIter++;
                }
                else
                {
                    text += $"     Перемена \n     {activity.Duration / 60} минут \n\n";
                }
            }

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: text,
                cancellationToken: cancellationToken);
        }
        
        async Task<Message> SendStream(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id, 
                text: "Ok... handling...");
            
            await using var stream = System.IO.File.OpenRead(@"D:\Distr\Films\Stream\output1.mp4");
            
            return await botClient.SendVideoAsync(
                chatId: message.Chat.Id,
                video: stream,
                supportsStreaming: true,
                cancellationToken: cancellationToken);
        }

        static async Task<Message> Usage(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            const string usage = "Usage:\n" +
                                 "/inline_keyboard - send inline keyboard\n" +
                                 "/keyboard    - send custom keyboard\n" +
                                 "/remove      - remove custom keyboard\n" +
                                 "/photo       - send a photo\n" +
                                 "/request     - request location or contact\n" +
                                 "/inline_mode - send keyboard with Inline Query";

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: usage,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }
    }

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable RCS1163 // Unused parameter.
    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore IDE0060 // Remove unused parameter
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);

        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }
}
