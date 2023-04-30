using GithubNotificationsTelegramBot;
using GithubNotificationsTelegramBot.Extensions;
using Octokit.Webhooks;
using Octokit.Webhooks.AspNetCore;
using Telegram.Bot;

var builder = WebApplication
    .CreateBuilder(args)
    .UseDefaultConfiguration(args);

builder.Services.AddLogging();

builder.Services
    .AddSingleton<WebhookEventProcessor, AppWebhookEventProcessor>()
    .AddHttpClient("telegram_bot_client")
    .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
    {
        var options = new TelegramBotClientOptions(builder.Configuration.GetValue<string>("TelegramBotToken")!);
        return new TelegramBotClient(options, httpClient);
    });

var app = builder.Build();

app
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", () => "Hello World!");
        endpoints.MapGitHubWebhooks(secret: builder.Configuration.GetValue<string>("GithubSecret")!);
    });

app.Run();