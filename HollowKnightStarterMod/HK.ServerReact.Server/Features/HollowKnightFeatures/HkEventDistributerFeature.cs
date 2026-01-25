using System.Text.Json;
using HK.Domain;
using HK.ServerReact.Server.Features.Hubs;

namespace HK.ServerReact.Server.Features.HollowKnightFeatures;

public class HkEventDistributerFeature() : IFeature
{
    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton<HkHubService>();
    }

    public void MapEndpoints(WebApplication app)
    {
        app.MapHub<HkHub>("/hubs/hk");
        app.MapPost("/api/hk/event",
            async (
                IServiceProvider serviceProvider,
                ILogger<HkEventDistributerFeature> logger,
                HkHubService hub,
                JsonDocument payload
            ) =>
            {
                JsonElement root = payload.RootElement;

                var eventJson = root.GetProperty("event");
                string className = eventJson
                    .GetProperty("ClassName")
                    .GetString()
                    ?? throw new BadHttpRequestException("ClassName missing");

                await (className switch
                {
                    nameof(DeathEvent) => HandleHkEvent(eventJson.Deserialize<DeathEvent>()),
                    nameof(RespawnEvent) => HandleHkEvent(eventJson.Deserialize<RespawnEvent>()),
                    nameof(HazardDeathEvent) => HandleHkEvent(eventJson.Deserialize<HazardDeathEvent>()),
                    _ => Task.CompletedTask,
                });

                async Task HandleHkEvent<T>(T? @event) where T : IEvent
                {
                    if (@event is null)
                    {
                        logger.LogError("Event should have been of type {EVENT_TYPE} but was null after deserialization", typeof(T));
                        return;
                    }

                    var subscribers = serviceProvider.GetRequiredService<IEnumerable<IHkSubscriber<T>>>();
                    var jobs = subscribers.Select(handler => handler.OnReceivedAsync(@event).AsTask())
                        .Concat([hub.SendEvent(@event)]);

                    await Task.WhenAll(jobs);
                }
            });
    }
}