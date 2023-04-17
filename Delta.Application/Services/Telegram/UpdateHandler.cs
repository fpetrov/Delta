using Delta.Application.Entities;
using Delta.Application.Messaging.Requests;
using Delta.Application.Repositories.Olimpiad;
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
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger, IUserService userService, IOlimpiadService olimpiadService)
    {
        _botClient = botClient;
        _logger = logger;
        _userService = userService;
        _olimpiadService = olimpiadService;
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
            { CallbackQuery: { } callbackQuery }           => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
            { InlineQuery: { } inlineQuery }               => BotOnInlineQueryReceived(inlineQuery, cancellationToken),
            { ChosenInlineResult: { } chosenInlineResult } => BotOnChosenInlineResultReceived(chosenInlineResult, cancellationToken),
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
            "/inline_keyboard" => SendInlineKeyboard(_botClient, message, cancellationToken),
            "/keyboard" => SendReplyKeyboard(_botClient, message, cancellationToken),
            "/remove" => RemoveKeyboard(_botClient, message, cancellationToken),
            "/request" => RequestContactAndLocation(_botClient, message, cancellationToken),
            "/inline_mode" => StartInlineQuery(_botClient, message, cancellationToken),
            "/throw" => FailingHandler(_botClient, message, cancellationToken),
            
            
            "/login" => SendLogin(_botClient, message, cancellationToken),
            
            "/olimpiad" => CreateOlimpiad(_botClient, message, cancellationToken),
            
            "/schedule" => SendSchedule(_botClient, message, cancellationToken),
            // "/homework" => SendSchedule(_botClient, message, cancellationToken),
            //
            // // TODO: Add support for ChatGPT via Render.com or WebProxy (Стас заплатит за прокси).
            // "/explain" => SendLogin(_botClient, message, cancellationToken),
            
            "/stream" => SendStream(_botClient, message, cancellationToken),
            _ => Usage(_botClient, message, cancellationToken)
        };
        
        var sentMessage = await action;
        _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);

        async Task<Message> CreateOlimpiad(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var request = new CreateOlimpiadRequest();
        }

        async Task<Message> SendLogin(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var dnevnikToken = messageText.Split(' ')[1];

            var request = new CreateUserRequest(message.Chat.Username!, message.Chat.Id, dnevnikToken, OlimpiadType.Math);

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
            for (var i = 0; i < schedule.Activities.Length; i++)
            {
                var activity = schedule.Activities[i];

                if (activity.Type == TypeEnum.Lesson)
                {
                    text += $"{lessonIter}. {activity.Lesson.SubjectName} \n     {activity.BeginTime} - {activity.EndTime} \n\n";
                    lessonIter++;
                }
                else
                    text += $"     Перемена \n     {activity.Duration / 60} минут \n\n";
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

        // Send inline keyboard
        // You can process responses in BotOnCallbackQueryReceived handler
        static async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await botClient.SendChatActionAsync(
                chatId: message.Chat.Id,
                chatAction: ChatAction.Typing,
                cancellationToken: cancellationToken);

            // Simulate longer running task
            await Task.Delay(500, cancellationToken);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("1.1", "11"),
                        InlineKeyboardButton.WithCallbackData("1.2", "12"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("2.1", "21"),
                        InlineKeyboardButton.WithCallbackData("2.2", "22"),
                    },
                });

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Choose",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }

        static async Task<Message> SendReplyKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[]
                {
                        new KeyboardButton[] { "1.1", "1.2" },
                        new KeyboardButton[] { "2.1", "2.2" },
                })
            {
                ResizeKeyboard = true
            };

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Choose",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }

        static async Task<Message> RemoveKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Removing keyboard",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }

        static async Task<Message> RequestContactAndLocation(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            ReplyKeyboardMarkup RequestReplyKeyboard = new(
                new[]
                {
                    KeyboardButton.WithRequestLocation("Location"),
                    KeyboardButton.WithRequestContact("Contact"),
                });

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Who or Where are you?",
                replyMarkup: RequestReplyKeyboard,
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

        static async Task<Message> StartInlineQuery(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            InlineKeyboardMarkup inlineKeyboard = new(
                InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode"));

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Press the button to start Inline Query",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }

#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable IDE0060 // Remove unused parameter
        static Task<Message> FailingHandler(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            throw new IndexOutOfRangeException();
        }
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RCS1163 // Unused parameter.
    }

    // Process Inline Keyboard callback data
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);

        await _botClient.AnswerCallbackQueryAsync(
            callbackQueryId: callbackQuery.Id,
            // text: $"Received {callbackQuery.Data}",
            cancellationToken: cancellationToken);

        var action = callbackQuery.Data switch
        {
            "yes" => HandleYesAnswer(_botClient, callbackQuery.Message!.Chat.Id, cancellationToken),
            "no" => _botClient.SendTextMessageAsync(
                chatId: callbackQuery.Message!.Chat.Id,
                text: $"Тогда введи правильные данные для входа",
                cancellationToken: cancellationToken),
            
            "technology" => HandleTechnologyAnswer(_botClient, callbackQuery.Message!.Chat.Id, cancellationToken),
            
            _ => _botClient.SendTextMessageAsync(
                chatId: callbackQuery.Message!.Chat.Id,
                text: $"Incorrect input",
                cancellationToken: cancellationToken)
        };
        
        var sentMessage = await action;

        static async Task<Message> HandleTechnologyAnswer(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            await botClient.SendChatActionAsync(
                chatId: chatId,
                chatAction: ChatAction.Typing,
                cancellationToken: cancellationToken);

            // Simulate longer running task
            await Task.Delay(500, cancellationToken);

            return await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Ты выбрал технологический профиль, теперь я буду уведомлять тебя об олимпиадах, которые тебе подходят. " +
                      $" Чтобы узнать больше о функционале, используй /help",
                cancellationToken: cancellationToken);
        }
        
        static async Task<Message> HandleYesAnswer(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            await botClient.SendChatActionAsync(
                chatId: chatId,
                chatAction: ChatAction.Typing,
                cancellationToken: cancellationToken);

            // Simulate longer running task
            await Task.Delay(500, cancellationToken);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Технологический (математика, информатика, физика)", "technology"),
                        InlineKeyboardButton.WithCallbackData("Социально-экономический (экономика, география, обществознание)", "economical"),
                        InlineKeyboardButton.WithCallbackData("Химико-биологический (химия, биология, нанотехнологии)", "chemistry"), 
                    }
                });

            return await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Супер! Теперь ты можешь пользоваться ботом. Выбери свой профиль, который тебе интересен",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
    }

    #region Inline Mode

    private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received inline query from: {InlineQueryFromId}", inlineQuery.From.Id);

        InlineQueryResult[] results = {
            // displayed result
            new InlineQueryResultArticle(
                id: "1",
                title: "TgBots",
                inputMessageContent: new InputTextMessageContent("hello"))
        };

        await _botClient.AnswerInlineQueryAsync(
            inlineQueryId: inlineQuery.Id,
            results: results,
            cacheTime: 0,
            isPersonal: true,
            cancellationToken: cancellationToken);
    }

    private async Task BotOnChosenInlineResultReceived(ChosenInlineResult chosenInlineResult, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);

        await _botClient.SendTextMessageAsync(
            chatId: chosenInlineResult.From.Id,
            text: $"You chose result with Id: {chosenInlineResult.ResultId}",
            cancellationToken: cancellationToken);
    }

    #endregion

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
