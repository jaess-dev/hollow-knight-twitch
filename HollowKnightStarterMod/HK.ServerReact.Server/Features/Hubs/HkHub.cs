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

public class HkHubService(IHubContext<HkHub> _hubContext)
{
    public async Task SendEvent<T>(T @event) where T : IEvent
    {
        await _hubContext.Clients.All.SendAsync(@event.ClassName, Serialize(@event));
    }

    private static string Serialize<T>(T @event) where T : IEvent
    {
        string @eventJson = @event is HazardDeathEvent death
            ? JsonSerializer.Serialize(new
            {
                ClassName = death.ClassName,
                HazardDeathDto = new
                {
                    HazardtypeDto = death.HazardDeathDto.HazardTypeDto.ToString()
                }
            })
            : JsonSerializer.Serialize(@event);
        return @eventJson;
    }
}