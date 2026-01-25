using HK.Domain;
using HollowKnightStarterMod.Domain.Model;

namespace HK.ServerReact.Server.Features.HollowKnightFeatures.RespawnMessageFeature;

public sealed class RespawnMessageFeature() : IFeature
{
    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton<PlayerDataService>();
        services.AddSingleton<IHkSubscriber<RespawnEvent>, PlayerDataService>();
    }

    public void MapEndpoints(WebApplication app)
    {
    }
}

public sealed class PlayerDataService(ILogger<PlayerDataService> logger) : IHkSubscriber<RespawnEvent>
{
    private PlayerDataDto? PlayerData { get; set; }

    public ValueTask OnReceivedAsync(RespawnEvent @event)
    {
        PlayerData = @event.playerData;
        logger.LogInformation("Player Data set {PlayerData}", PlayerData);
        return ValueTask.CompletedTask;
    }
}