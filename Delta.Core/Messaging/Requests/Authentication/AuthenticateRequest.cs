namespace Delta.Core.Messaging.Requests.Authentication;

public record AuthenticateRequest(
    string Name,
    string Password,
    string Fingerprint
);