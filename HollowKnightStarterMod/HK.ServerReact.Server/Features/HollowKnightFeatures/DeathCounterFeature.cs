using HK.Domain;
using HK.ServerReact.Server.Services;
using HK.ServerReact.Server.Util;

namespace HK.ServerReact.Server.Features.HollowKnightFeatures;

public sealed class DeathCounterFeature() : IFeature
{
    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton<IHkSubscriber<RespawnEvent>, RespawnSub>();
    }

    public void MapEndpoints(WebApplication app) { }
}

public sealed class RespawnSub(ILogger<RespawnSub> logger) : IHkSubscriber<RespawnEvent>
{
    public ValueTask OnReceivedAsync(RespawnEvent @event)
    {
        logger.LogInformation("Received a death counter of {DEATH_COUNTER}", @event.PlayerData.DeathCount);
        // @event.playerData.deathCount;
        return ValueTask.CompletedTask;
    }
}

