using Delta.Core.Entities.Authentication;

namespace Delta.Core.Messaging.Responses.Authentication;

public record AuthenticateResponse(
    int Id,
    string Jwt,
    RefreshToken RefreshToken
);