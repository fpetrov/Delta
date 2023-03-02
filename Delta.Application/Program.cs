using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Showcast.Core.Repositories.User;
using Showcast.Core.Services.Security;
using Showcast.Infrastructure;
using Showcast.Infrastructure.Contexts;
using Showcast.Infrastructure.Repositories.User;
using Showcast.Infrastructure.Services.Http;
using Showcast.Infrastructure.Services.Security;
using Showcast.TelegramBot;
using Showcast.TelegramBot.Extensions;
using Showcast.TelegramBot.Services;
using Showcast.TelegramBot.Services.Telegram;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("telegram_bot_client")
    .AddTypedClient<ITelegramBotClient>(httpClient =>
    {
        var token = builder.Configuration["TelegramBotToken"]!;
        
        return new TelegramBotClient(token, httpClient);
    });

builder.Services.AddMovieService();
// builder.Services.AddHostedService<TelegramHostedService>();

builder.Services.AddAutoMapper(typeof(IAssemblyMarker));
builder.Services.AddMediatR(typeof(IAssemblyMarker));

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<ReceiverService>();

var connectionString = builder.Configuration.GetConnectionString("SupabasePostgreSQL");

builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IHashService, HashService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


builder.Services.AddHostedService<PollingService>();

var app = builder.Build();

app.Run();


// using System.Text;
//
// var path = @"D:\Projects\Web\Showcast\Showcast.RecommendationSystem\Notebooks\RecSys\Datasets\movies.csv";
//
// var file = File.ReadAllLines(path);
//
// for (var i = 1; i < file.Length; i++)
// {
//     var split = file[i].Split(',');
//
//     var filteredTitle = string.Join(" ", split[1].Split(' ')[..^1]);
//
//     var newLine = $"{split[0]},{filteredTitle},{split[2]}";
//
//     file[i] = newLine;
// }
//
// File.WriteAllLines(path, file);