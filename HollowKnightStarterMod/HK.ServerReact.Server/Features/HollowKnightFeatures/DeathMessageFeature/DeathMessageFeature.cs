using HK.Domain;
using HK.ServerReact.Server.Features.HollowKnightFeatures;
using HK.ServerReact.Server.Features.HollowKnightFeatures.DeathMessageFeature.Models;
using HK.ServerReact.Server.Features.HollowKnightFeatures.DeathMessageFeature.Services;

namespace HK.ServerReact.Server.Features.HollowKnightFeatures.DeathMessageFeature;

public sealed class DeathMessageFeature(params string[] deathMessages) : IFeature
{
    private readonly DeathMessageContainer _messages = new(deathMessages);

    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton<DeathMessageContainer>(sp =>
        {
            if (sp.GetRequiredService<IConfiguration>().GetSection("Hk:DeathMessages").Get<string[]>() is not { } messages)
            {
                throw new ArgumentException("No death messages provided!");
            }

            return new(messages);
        });
        services.AddSingleton<DeathMessageService>();
        services.AddSingleton<IHkSubscriber<DeathEvent>, DeathMessageService>();
    }

    public void MapEndpoints(WebApplication app)
    {
    }
}