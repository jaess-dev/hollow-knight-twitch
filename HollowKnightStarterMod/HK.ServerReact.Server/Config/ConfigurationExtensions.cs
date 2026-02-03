using HK.ServerReact.Server.Features.TwClient.Subs;
using HK.ServerReact.Server.Services;
using HK.ServerReact.Server.Services.TwitchConnection;
using twitch_con.TwClient;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;

namespace HK.ServerReact.Server.Config;

internal static class ConfigurationExtensions
{
    extension(IServiceCollection self)
    {
        internal IServiceCollection ConfigureServices()
        {
            self.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:54024",  // React dev server
                            "https://localhost:54024"  // React dev server
                                                       // ,"https://your-production-frontend.com"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); // Required for SignalR
                });
            });

            self.AddControllers();
            self.AddOpenApi();
            self.AddSignalR();
            self.AddHttpClient();

            self.AddSingleton(static sp =>
            {
                var url = sp.GetRequiredService<IConfiguration>().GetSection("WebSocket").Get<string>() ?? "ws://localhost:8080";
                return new WebSocketService(new Uri(url), sp.GetRequiredService<ILogger<WebSocketService>>());
            });
            self.AddSingleton<IWebSocketService>(sp => sp.GetRequiredService<WebSocketService>());
            self.AddSingleton<IBotMessageSender, BotWsConnectorService>();
            return self;
        }

        internal IServiceCollection ConfigureTwitchBot(
            string targetChannel,
            string botName,
            string clientSecret,
            string clientId,
            bool useBaseSubscription = true)
        {
            self.AddSingleton<ITokenStore>(sp => new TwitchTokenJsonStore("Resources/tokens.json"));

            self.AddKeyedSingleton(DiKeys.TARGET_CHANNEL, targetChannel);
            self.AddKeyedSingleton(DiKeys.BOT_NAME, botName);
            self.AddKeyedSingleton(DiKeys.CLIENT_SECRET, clientSecret);
            self.AddKeyedSingleton(DiKeys.CLIENT_ID, clientId);

            if (useBaseSubscription)
            {
                self.AddSingleton<ITwitchClientSubscriber, BaseSubscriptions>();
            }

            self.AddSingleton<ITwitchClient, TwitchClient>(sp => new(protocol: TwitchLib.Client.Enums.ClientProtocol.WebSocket));
            // self.AddSingleton<IBotMessageSender, TwitchService>();
            self.AddHostedService<TwitchClientHostedService>();
            return self;
        }
    }
}