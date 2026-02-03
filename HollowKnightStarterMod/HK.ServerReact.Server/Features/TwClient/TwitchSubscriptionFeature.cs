using HK.ServerReact.Server.Features.TwClient.Hubs;
using HK.ServerReact.Server.Features.TwClient.Subs;
using twitch_con.TwClient;

namespace HK.ServerReact.Server.Features.TwClient;

public class TwitchSubscriptionFeature(Type[]? messageHandlers = null) : IFeature
{
    public readonly List<Type> _messageHandlers = (messageHandlers ?? []).Select(t =>
    {
        return typeof(IMessageHandler).IsAssignableFrom(t)
            ? t
            : throw new ArgumentException($"The provided type was not of {nameof(IMessageHandler)}");
    }).ToList();
    public void AddMessageHandler<T>() where T : IMessageHandler => _messageHandlers.Add(typeof(T));


    public void AddServices(IServiceCollection services)
    {
        foreach (var t in _messageHandlers) services.AddSingleton(typeof(IMessageHandler), t);

        services.AddSingleton<ITwitchClientSubscriber, BaseSubscriptions>();
        services.AddSingleton<TwitchHubService>();
    }

    public void MapEndpoints(WebApplication app)
    {
        app.MapHub<TwitchHub>("/hubs/twitch");
    }
}