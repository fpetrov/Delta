namespace Delta.Dnevnik.Authenticators;

public class TokenAuthenticator : IDnevnikAuthenticator
{
    private readonly string _token;
    
    public TokenAuthenticator(string token)
    {
        _token = token;
        
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token), "Authenticator token is empty!");
    }

    public string Authenticate() => _token;
}