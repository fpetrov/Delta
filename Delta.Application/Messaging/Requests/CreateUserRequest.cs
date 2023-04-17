using Delta.Application.Entities;

namespace Delta.Application.Messaging.Requests;

public record CreateUserRequest(string Name, long TelegramId, string DnevnikToken, OlimpiadType OlimpiadType);