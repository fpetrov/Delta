namespace Delta.Application.Messaging.Requests;

public record GetScheduleRequest(long TelegramId, DateTime Date);