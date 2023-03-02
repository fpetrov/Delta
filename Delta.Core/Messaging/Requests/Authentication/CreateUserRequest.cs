namespace Delta.Core.Messaging.Requests.Authentication;

public record CreateUserRequest(
    string Name,
    string Password,
    string Fingerprint,
    long? TelegramId = 0
);