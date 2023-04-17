using Delta.Dnevnik.Authenticators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Delta.Dnevnik;

public record DnevnikOptions(
    string Url,
    IDnevnikAuthenticator Authenticator,
    ILoggerFactory LoggerFactory,
    TimeSpan RequestTimeout)
{
    public static DnevnikOptions Default = new DnevnikOptions(
        Url: "https://dnevnik.mos.ru/mobile/api",
        Authenticator: new TokenAuthenticator("Token"),
        LoggerFactory: NullLoggerFactory.Instance,
        RequestTimeout: TimeSpan.FromSeconds(5));
    
    public IDnevnikAuthenticator Authenticator { get; init; }
}