using Delta.Dnevnik;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Delta.Application;

/// <summary>
/// Service for automated newsletter for bot users.
/// </summary>
public class SubscribersNotifierService : BackgroundService
{
    private readonly ILogger<SubscribersNotifierService> _logger;
    private readonly IDnevnikPlainConnection _dnevnikConnection;
    private readonly PeriodicTimer _periodicTimer;

    public SubscribersNotifierService(ILogger<SubscribersNotifierService> logger, IDnevnikPlainConnection dnevnikConnection)
    {
        _logger = logger;
        _dnevnikConnection = dnevnikConnection;
        _periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(10));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (await _periodicTimer.WaitForNextTickAsync(cancellationToken))
        {
            _logger.LogInformation("Tick");

            //var student = await _dnevnikConnection.GetStudentAsync("eyJhbGciOiJSUzI1NiJ9.eyJzdWIiOiIyNzI2NDQxIiwiYXVkIjoiMjoxIiwibmJmIjoxNjgxNjU3MjY0LCJtc2giOiI3YTEyZGRiYS1lMzk1LTQ5OTEtOTNkNi1iNzhmNGZlOWY4ZDYiLCJhdGgiOiJzdWRpciIsImlzcyI6Imh0dHBzOlwvXC9zY2hvb2wubW9zLnJ1IiwicmxzIjoiezE6WzE4MzoxNjpbXSwzMDo0OltdLDQwOjE6W10sMjA6MjpbXV19IiwiZXhwIjoxNjgyNTIxMjY0LCJpYXQiOjE2ODE2NTcyNjQsImp0aSI6IjJiOTM1Njg0LTczMWEtNGI1ZC05MzU2LTg0NzMxYWRiNWQ1MSIsInNzbyI6ImY4ZGE0MWExLTM0ZTAtNGFmMi1hMzdmLTdhNTE0MmUyZDQxNCJ9.i11vyeBzw1zsA9ZHF190nGMAsdX0KYk10vp2c1pCYLfmtAzcERzyxu5kBDzoyRwYfqTrqDfQ3PTNQhktWFMjjU_UnXoXNw57QmJCCOEO8Tu-bUYCvimJdgNFpnmtbw_bWkggP5b-LcxrVGH-iQr-vjkwz_DN6Os2v6EW6v7xnStGXvrmJsO4kczJApFFHgYLWx_H8kN94TGIlaQZ4VKSzhzcQcfUkGChqiS4KQtR8h1lS3C44d_90aCVLZiBJUdu1_Wein8-pOom25auT6WYgI5he02oqxEycbV92h3yh1u_2Xwqv_iSxpcdmdtHeVY2HIL2diHHJKCCCLMqoANNtg");
            
            //_logger.LogInformation(student.Profile.FirstName);
        }
    }
}