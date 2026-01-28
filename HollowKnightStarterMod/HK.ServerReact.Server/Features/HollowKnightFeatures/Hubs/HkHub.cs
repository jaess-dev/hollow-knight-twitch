using System.Text.Json;
using HK.Domain;
using HollowKnightStarterMod.Domain.Model;
using Microsoft.AspNetCore.SignalR;

namespace HK.ServerReact.Server.Features.Hubs;



public class HkHub : Hub
{
    public async Task SendEvent(IEvent @event)
    {
        await Clients.All.SendAsync(@event.ClassName, JsonSerializer.Serialize(@event));
    }
}

public class HkHubService(IHubContext<HkHub> _hubContext, ILogger<HkHubService> logger)
{
    public async Task SendEvent<T>(T @event) where T : IEvent
    {
        logger.LogInformation("Sending event {EVENT}", @event);
        await _hubContext.Clients.All.SendAsync(@event.ClassName, Serialize(@event));
    }

    private static object Serialize<T>(T @event) where T : IEvent
    {
        return @event is HazardDeathEvent death
            ? new
            {
                ClassName = death.ClassName,
                HazardDeathDto = new
                {
                    HazardTypeDto = death.HazardDeathDto.HazardTypeDto.ToString()
                }
            }
            : @event;
    }
}