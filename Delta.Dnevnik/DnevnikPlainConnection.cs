using System.Text.Json;
using System.Web;
using Delta.Dnevnik.Models;
using Microsoft.Extensions.Options;

namespace Delta.Dnevnik;

public class DnevnikPlainConnection : IDnevnikPlainConnection
{
    private const string AuthenticationTokenHeader = "Auth-Token";
    private const string StudentIdHeader = "Profile-Id";
    
    private readonly HttpClient _httpClient;
    private readonly DnevnikOptions _options;

    public DnevnikPlainConnection(IOptions<DnevnikOptions>? options = default, HttpClient? httpClient = default)
    {
        _options = options?.Value ?? DnevnikOptions.Default;
        _httpClient = httpClient ?? new HttpClient();
    }

    public async Task<Schedule?> GetScheduleAsync(string token, long profileId, string dateTime)
    {
        ApplyAuthenticationHeader(token);
        ApplyStudentIdHeader(profileId.ToString());

        var json = await _httpClient.GetStringAsync(_options.Url + $"/schedule?student_id={profileId}&date={dateTime}");

        return JsonSerializer.Deserialize<Schedule>(json);
    }

    public async Task<Student?> GetStudentAsync(string token)
    {
        ApplyAuthenticationHeader(token);

        var json = await _httpClient.GetStringAsync(_options.Url + "/profile/");

        return JsonSerializer.Deserialize<Student>(json);
    }

    private void ApplyAuthenticationHeader(string token)
    {
        if (!_httpClient.DefaultRequestHeaders.Contains(AuthenticationTokenHeader)) 
            _httpClient.DefaultRequestHeaders.Add(AuthenticationTokenHeader, token);
    }

    private void ApplyStudentIdHeader(string profileId)
    {
        if (!_httpClient.DefaultRequestHeaders.Contains(StudentIdHeader)) 
            _httpClient.DefaultRequestHeaders.Add(StudentIdHeader, profileId);
    }
}