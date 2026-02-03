using HK.ServerReact.Server.Features;
using HK.ServerReact.Server.Features.TwClient.Hubs;
using HK.ServerReact.Server.Features.TwClient.Subs;
using twitch_con.TwClient;

namespace HK.ServerReact.Server.Features.TwClient;

public class TwitchSubscriptionFeature : IFeature
{
    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton<ITwitchClientSubscriber, BaseSubscriptions>();
        services.AddSingleton<TwitchHubService>();
    }

    public void MapEndpoints(WebApplication app)
    {
        app.MapHub<TwitchHub>("/hubs/twitch");
    }
}