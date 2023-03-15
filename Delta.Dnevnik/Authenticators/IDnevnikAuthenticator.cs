namespace Delta.Dnevnik.Authenticators;

public interface IDnevnikAuthenticator
{
    public Task<bool> Authenticate();
}