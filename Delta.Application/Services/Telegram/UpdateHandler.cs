using MediatR;
using Microsoft.Extensions.Logging;
using Showcast.Core.Repositories.User;
using Showcast.Core.Services.Http;
using Showcast.Infrastructure.Messaging.Authentication.Commands;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace Showcast.TelegramBot.Services.Telegram;

public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandler> _logger;
    private readonly IMovieService _movieService;
    private readonly IUserRepository _userRepository;

    public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger, IMovieService movieService, IUserRepository userRepository)
    {
        _botClient = botClient;
        _logger = logger;
        _movieService = movieService;
        _userRepository = userRepository;
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
            "/relative" => SendRelativeMovies(_botClient, message, cancellationToken),
            "/recommendations" => SendRecommendations(_botClient, message, cancellationToken),
            "/login" => SendLoginMessage(_botClient, message, cancellationToken),
            "/like" => AddLikedMovies(_botClient, message, cancellationToken),
            "/likes" => SendLikedMovies(_botClient, message, cancellationToken),
            "/info" => SendMovieInfo(_botClient, message, cancellationToken),
            "/stream" => SendStream(_botClient, message, cancellationToken),
            _ => Usage(_botClient, message, cancellationToken)
        };
        var sentMessage = await action;
        _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);

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
        
        async Task<Message> SendLikedMovies(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindAsync(user => user.TelegramId == message.From.Id);
            
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: string.Join(", ", user.LikedMovies),
                cancellationToken: cancellationToken);
        }

    async Task<Message> SendRecommendations(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindAsync(user => user.TelegramId == message.From.Id);
            var recommendations = _movieService.GetRecommendations(user);

            await foreach (var relative in recommendations)
            {
                var caption =
                    $"***{relative.Title}***, ***{relative.Rating} / 10*** \n \n" +
                    $"{relative.Plot}       \n" +
                    "\n" +
                    $"***Genres***: {relative.Genre} \n \n" +
                    $"***Released in {relative.Year}***";

                await botClient.SendPhotoAsync(chatId: message.Chat.Id,
                    photo: new InputOnlineFile(new Uri(relative.PosterAddress)),
                    caption: caption,
                    ParseMode.Markdown,
                    cancellationToken: cancellationToken);
            }
            
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"А вот и ваши рекомендации",
                cancellationToken: cancellationToken);
        }
        
        async Task<Message> AddLikedMovies(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var text = string.Join(' ', message.Text.Split(' ')[1..]);

            var titles = text.Split(',');
            
            var user = await _userRepository.FindAsync(user => user.TelegramId == message.From.Id);
            
            user.LikedMovies.AddRange(titles);
            
            await _userRepository.UpdateAsync(user, cancellationToken);
            
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Эти фильмы были добавлены в понравившиеся",
                cancellationToken: cancellationToken);
        }
        
        async Task<Message> SendMovieInfo(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var title = string.Join(" ", message.Text.Split(' ')[1..]);

            var movie = await _movieService.GetMovie(title);

            var caption =
                $"***{movie.Title}***, ***{movie.Rating} / 10*** \n \n" +
                $"`{movie.Plot}`       \n" +
                "\n" +
                $"***Genres:*** {movie.Genre} \n \n" +
                $"***BoxOffice:*** {movie.BoxOffice} \n" +
                $"***Director:*** {movie.Director} \n" +
                $"***Actors:*** {movie.Actors} \n" +
                $"***Released in {movie.Year}*** \n";

            return await botClient.SendPhotoAsync(chatId: message.Chat.Id,
                photo: new InputOnlineFile(new Uri(movie.PosterAddress)),
                caption: caption,
                ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }

        async Task<Message> SendLoginMessage(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var payload = message.Text.Split(' ')[1..];
            
            var user = await _userRepository.SignUpAsync(new(
                payload[0], payload[1], message.From!.Id.ToString(), message.From.Id));
            
            
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Вы вошли в аккаунт! Теперь вы можете добавить понравившиеся фильмы, чтобы бот мог вывести персональные рекомендации. Ваш Id: {user.Id}",
                cancellationToken: cancellationToken);
        }
        
        async Task<Message> SendRelativeMovies(ITelegramBotClient botClient, Message message,
            CancellationToken cancellationToken)
        {
            var title = string.Join(" ", message.Text.Split(' ')[1..]);

            var relatives = _movieService.GetRelative(title);

            await foreach (var relative in relatives)
            {
                var caption =
                    $"***{relative.Title}***, ***{relative.Rating} / 10*** \n \n" +
                    $"{relative.Plot}       \n" +
                    "\n" +
                    $"***Genres***: {relative.Genre} \n \n" +
                    $"***Released in {relative.Year}***";

                await botClient.SendPhotoAsync(chatId: message.Chat.Id,
                    photo: new InputOnlineFile(new Uri(relative.PosterAddress)),
                    caption: caption,
                    ParseMode.Markdown,
                    cancellationToken: cancellationToken);
            }

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "That's all!",
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
            text: $"Received {callbackQuery.Data}",
            cancellationToken: cancellationToken);

        await _botClient.SendTextMessageAsync(
            chatId: callbackQuery.Message!.Chat.Id,
            text: $"Received {callbackQuery.Data}",
            cancellationToken: cancellationToken);
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
