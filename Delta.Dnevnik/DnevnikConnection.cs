using Delta.Dnevnik.Authenticators;
using Delta.Dnevnik.Models;
using Microsoft.Extensions.Logging;

namespace Delta.Dnevnik;

/// <summary>
/// TODO: Create Factory class for Dnevnik Connection.
/// </summary>
public class DnevnikConnection : IDnevnikConnection, IDisposable
{
    private readonly IDnevnikAuthenticator _dnevnikAuthenticator;
    private readonly DnevnikOptions _options;
    private readonly HttpClient _httpClient;
    private readonly ILogger<DnevnikConnection> _logger;
    
    public DnevnikConnection(DnevnikOptions options, HttpClient? httpClient = default)
    {
        _options = options;

        if (options.Authenticator is null)
            throw new ArgumentNullException(nameof(options.Authenticator));

        _dnevnikAuthenticator = options.Authenticator;

        _logger = options.LoggerFactory.CreateLogger<DnevnikConnection>();

        _httpClient = httpClient ?? new HttpClient();
    }

    public Task<Schedule> GetScheduleAsync(DateTime dateTime)
    {
        throw new NotImplementedException();
    }

    public Task<Student> GetStudentAsync()
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        _logger.LogInformation("Disposing Dnevnik connection!");
        
        _httpClient.Dispose();
    }
}