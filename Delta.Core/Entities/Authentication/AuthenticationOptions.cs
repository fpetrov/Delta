namespace Delta.Core.Entities.Authentication;

public record AuthenticationOptions(
    string SecurityKey,
    string Issuer,
    string Audience,
    TimeSpan TokenLifetime,
    TimeSpan RefreshTokenLifetime
)
{
    public static AuthenticationOptions Default = new(
        "chdopqazx54effke456vd439gfj5326c7b1hjw=578flhaiop23",
        "Showcast.Application",
        "Showcast.UI",
        TimeSpan.FromMinutes(60),
        TimeSpan.FromDays(30)
    );

    public AuthenticationOptions() : this(Default)
    {
        
    }
}