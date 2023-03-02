using System.Linq.Expressions;
using Showcast.Core.Messaging.Requests.Authentication;
using Showcast.Core.Messaging.Responses.Authentication;

namespace Showcast.TelegramBot.Services.User;

public interface IUserService
{
    public Task<bool> Authenticate(
        AuthenticateRequest request,
        CancellationToken cancellationToken = default);

    public Task<bool> Create(
        CreateUserRequest request,
        CancellationToken cancellationToken = default);

    public Task<AuthenticateResponse?> RefreshToken(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default);

    public Task<bool> UpdateLikedMovies(string[] movies);

    public Task<bool> RevokeToken(
        RevokeTokenRequest request,
        CancellationToken cancellationToken = default);
}