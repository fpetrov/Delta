using System.Linq.Expressions;
using Delta.Core.Messaging.Requests.Authentication;
using Delta.Core.Messaging.Responses.Authentication;

namespace Delta.Core.Repositories.User;

public interface IUserRepository : IRepository<Entities.Authentication.User>
{
    public Task<AuthenticateResponse?> SignInAsync(
        AuthenticateRequest request,
        Func<Entities.Authentication.User, bool> passwordVerifier,
        CancellationToken cancellationToken = default);

    public Task<AuthenticateResponse?> SignUpAsync(
        CreateUserRequest request,
        Expression<Func<Entities.Authentication.User, bool>>? duplicatePredicate = default,
        CancellationToken cancellationToken = default);

    public Task<AuthenticateResponse?> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default);

    public Task<bool> RevokeTokenAsync(
        RevokeTokenRequest request,
        CancellationToken cancellationToken = default);
}