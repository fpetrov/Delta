using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    private const string AuthenticationTokenHeader = "Auth-Token";
    
    public DnevnikConnection(DnevnikOptions options, HttpClient? httpClient = default)
    {
        _options = options;

        if (options.Authenticator is null)
            throw new ArgumentNullException(nameof(options.Authenticator));

        _dnevnikAuthenticator = options.Authenticator;

        _logger = options.LoggerFactory.CreateLogger<DnevnikConnection>();

        _httpClient = httpClient ?? new HttpClient();
        
        _httpClient.DefaultRequestHeaders.Add(AuthenticationTokenHeader, _dnevnikAuthenticator.Authenticate());
    }

    public async Task<Schedule> GetScheduleAsync(DateTime dateTime)
    {
        _logger.LogInformation("Fetching student schedule");

        var json = await _httpClient.GetStringAsync(_options.Url + "/profile/");

        return new Schedule();
    }

    public async Task<Student?> GetStudentAsync()
    {
        _logger.LogInformation("Fetching student data");

        var json = await _httpClient.GetStringAsync(_options.Url + "/profile/");

        return JsonSerializer.Deserialize<Student>(json);
    }
    
    public void Dispose()
    {
        _logger.LogInformation("Disposing Dnevnik connection!");
        
        _httpClient.Dispose();
    }
}