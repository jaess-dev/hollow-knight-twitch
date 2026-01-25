using HK.Domain;
using HK.ServerReact.Server.Services;
using HK.ServerReact.Server.Util;

namespace HK.ServerReact.Server.Features.HollowKnightFeatures;

public sealed class HazardResponseFeature(Dictionary<string, string[]>? hazardMessages = null) : IFeature
{
    private readonly HazardMessageContainer _messages = new(hazardMessages ?? []);

    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton<HazardMessageContainer>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<HazardResponseFeature>>();
            if (sp.GetRequiredService<IConfiguration>().GetSection("Hk:HazardMessages").Get<Dictionary<string, string[]>>() is not { } messages)
            {
                logger.LogInformation("No Hazard messages found");
                return new([]);
            }

            logger.LogInformation("Hazard messages loaded {MESSAGES}", messages);
            return new(messages.ToDictionary(kv => kv.Key.ToLower(), kv => kv.Value));
        });
        services.AddSingleton<HazardResponseMessage>();
        services.AddSingleton<IHkSubscriber<HazardDeathEvent>, HazardResponseMessage>();
    }

    public void MapEndpoints(WebApplication app)
    {
    }
}

public sealed class HazardResponseMessage(
    BotConnectorServices botConnector,
    HazardMessageContainer messages) : IHkSubscriber<HazardDeathEvent>
{
    private readonly HazardMessageContainer _messages = messages;

    public async ValueTask OnReceivedAsync(HazardDeathEvent @event)
    {
        var str = @event.HazardDeathDto.HazardTypeDto.ToString();
        await botConnector.SendChatMessageAsync(
            _messages[str].AsSpan().ChooseRandom()
        );
    }
}


public sealed class HazardMessageContainer(Dictionary<string, string[]> messages)
{
    private readonly Dictionary<string, string[]> _messages = messages;

    public string[] this[string idx] => _messages[idx.ToLower()];
}