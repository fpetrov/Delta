using Delta.Application.Entities;

namespace Delta.Application.Messaging.Requests;

public record AddNotificationRequest(long TelegramId, Notification Notification);