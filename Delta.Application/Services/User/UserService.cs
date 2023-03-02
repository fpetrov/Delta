using Showcast.Core.Messaging.Requests.Authentication;
using Showcast.Core.Messaging.Responses.Authentication;

namespace Showcast.TelegramBot.Services.User;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _httpClient.DefaultRequestHeaders.Add("Telegram-Id", "");
    }

    public Task<bool> Authenticate(AuthenticateRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Create(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<AuthenticateResponse?> RefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateLikedMovies(string[] movies)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RevokeToken(RevokeTokenRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}