using Delta.Core.Entities.Authentication;

namespace Delta.Core.Services.Security;

public interface ITokenService
{
    public (string token, RefreshToken refreshToken) GenerateTokenPair(User user, string fingerprint);
}