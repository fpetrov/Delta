namespace Delta.Dnevnik.Authenticators;

public interface IDnevnikAuthenticator
{
    /// <summary>
    /// Authenticate in MOS.RU API to fetch authentication token.
    /// </summary>
    /// <returns>Authentication token.</returns>
    public string Authenticate();
}