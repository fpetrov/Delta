namespace Delta.Application.Messaging.Requests;

public record GetHomeworkRequest(long TelegramId, DateTime Date);