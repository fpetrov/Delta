using Delta.Application;
using Delta.Application.Contexts;
using Delta.Application.Extensions;
using Delta.Application.Services.Telegram;
using Delta.Dnevnik;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("telegram_bot_client")
    .AddTypedClient<ITelegramBotClient>(httpClient =>
    {
        var token = builder.Configuration["TelegramBotToken"]!;

        return new TelegramBotClient(token, httpClient);
    });

builder.Services.AddHttpClient("dnevnik_client")
    .AddTypedClient<IDnevnikPlainConnection>(httpClient =>
    {
        var dnevnik = new DnevnikPlainConnection(httpClient: httpClient);

        return dnevnik;
    });

var connectionString = builder.Configuration.GetConnectionString("SupabaseConnectionString");

builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddAutoMapper(typeof(IAssemblyMarker));

builder.Services.AddUserService();
builder.Services.AddOlimpiadService();

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<ReceiverService>();

builder.Services.AddHostedService<SubscribersNotifierService>();
builder.Services.AddHostedService<PollingService>();

var app = builder.Build();

app.Run();