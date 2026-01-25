using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;

namespace HK.ServerReact.Server.Features.CounterStrikeFeatures;


public class CsHub : Hub
{
    public async Task SendEvent(ICsEvent @event)
    {
        await Clients.All.SendAsync(@event.EventName, (@event));
    }
}

public class CsHubService(IHubContext<CsHub> _hubContext)
{
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNamingPolicy = null,
        IncludeFields = true, // Include fields
        WriteIndented = false,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };

    public async Task SendEvent<T>(T @event) where T : ICsEvent
    {
        var eventSerialized = JsonSerializer.SerializeToDocument(@event, options);
        await _hubContext.Clients.All.SendAsync(@event.EventName, eventSerialized);
    }

    internal static object Serialize<T>(T @event) where T : ICsEvent
    {
        return @event;
    }
}