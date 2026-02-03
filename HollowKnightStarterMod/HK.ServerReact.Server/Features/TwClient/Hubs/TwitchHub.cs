using System.Text.Json;
using HK.Domain;
using HollowKnightStarterMod.Domain.Model;
using Microsoft.AspNetCore.SignalR;

namespace HK.ServerReact.Server.Features.TwClient.Hubs;

public class TwitchHub : Hub
{
    public async Task SendEvent(ITwitchHubEvent @event)
    {
        await Clients.All.SendAsync(@event.ClassName, JsonSerializer.Serialize(@event));
    }
}

public class TwitchHubService(IHubContext<TwitchHub> hubContext, ILogger<TwitchHubService> logger)
{
    private readonly ILogger<TwitchHubService> _logger = logger;
    private readonly IHubContext<TwitchHub> _hub = hubContext;

    public async Task SendEvent<T>(T @event) where T : ITwitchHubEvent
    {
        _logger.LogInformation("Sending event {EVENT}", @event);
        await _hub.Clients.All.SendAsync(@event.ClassName, Serialize(@event));
    }

    private static object Serialize<T>(T @event) where T : ITwitchHubEvent
    {
        return @event;
    }
}