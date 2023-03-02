using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Showcast.Core.Services.Http;
using Showcast.Infrastructure.Services.Http;

namespace Showcast.TelegramBot;

public class TelegramHostedService : BackgroundService
{
    private readonly IMovieService _movieService;
    private readonly ILogger<TelegramHostedService> _logger;

    public TelegramHostedService(IMovieService movieService, ILogger<TelegramHostedService> logger)
    {
        _movieService = movieService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // var relatives = _movieService.GetRelative("Prisoners (2013)");
            //
            // await foreach (var relative in relatives.WithCancellation(stoppingToken))
            // {
            //     _logger.LogInformation(relative!.Actors);
            // }

            // var movie = await _movieDbService.GetByTitle("Prisoners (2013)");
            //
            // _logger.LogInformation(movie?.Plot);

            await Task.Delay(2000, stoppingToken);
        }
    }
}