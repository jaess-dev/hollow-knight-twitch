using System.Text.Json;
using HK.Domain;

namespace HK.ServerReact.Server.Features.HollowKnightFeatures;

public class HkEventDistributerFeature() : IFeature
{
    public void AddServices(IServiceCollection services)
    {
    }

    public void MapEndpoints(WebApplication app)
    {
        app.MapPost("/api/hk/event",
            async (
                IServiceProvider serviceProvider,
                ILogger<HkEventDistributerFeature> logger,
                JsonDocument payload
            ) =>
            {
                JsonElement root = payload.RootElement;

                var eventJson = root.GetProperty("event");
                string className = eventJson
                    .GetProperty("ClassName")
                    .GetString()
                    ?? throw new BadHttpRequestException("ClassName missing");

                switch (className)
                {
                    case nameof(DeathEvent):
                        HandleHkEvent(eventJson.Deserialize<DeathEvent>());
                        break;
                }

                void HandleHkEvent<T>(T? @event) where T : IEvent
                {
                    if (@event is null)
                    {
                        logger.LogError("Event should have been of type {EVENT_TYPE} but was null after deserialization", typeof(T));
                        return;
                    }

                    foreach (var handler in serviceProvider.GetRequiredService<IEnumerable<IHkSubscriber<T>>>())
                    {
                        handler.OnReceived(@event);
                    }
                }
            });
    }
}