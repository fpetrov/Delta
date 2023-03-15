using Delta.Application.Extensions;
using Delta.Application.Services.Telegram;
using Delta.Dnevnik;
using Delta.Dnevnik.Authenticators;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;


var options = DnevnikOptions.Default with
{
    Authenticator = new TokenAuthenticator("Some-Token")
};

var dnevnik = new DnevnikConnection(options);

// var builder = Host.CreateApplicationBuilder(args);
//
// builder.Services.AddHttpClient("telegram_bot_client")
//     .AddTypedClient<ITelegramBotClient>(httpClient =>
//     {
//         var token = builder.Configuration["TelegramBotToken"]!;
//         
//         return new TelegramBotClient(token, httpClient);
//     });
//
// builder.Services.AddScoped<UpdateHandler>();
// builder.Services.AddScoped<ReceiverService>();
//
// builder.Services.AddMovieService();
//  builder.Services.AddHostedService<TelegramHostedService>();
//
//  builder.Services.AddAutoMapper(typeof(IAssemblyMarker));
//  builder.Services.AddMediatR(typeof(IAssemblyMarker));
//
//  
//
//  var connectionString = builder.Configuration.GetConnectionString("SupabasePostgreSQL");
//
//  builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connectionString));
//
//  builder.Services.AddScoped<IHashService, HashService>();
//  builder.Services.AddScoped<ITokenService, TokenService>();
//  builder.Services.AddScoped<IUserRepository, UserRepository>();
//
//
// builder.Services.AddHostedService<PollingService>();
//
// var app = builder.Build();
//
// app.Run();
