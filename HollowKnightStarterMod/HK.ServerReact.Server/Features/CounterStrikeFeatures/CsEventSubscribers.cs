
using System.Diagnostics.Tracing;
using CounterStrike2GSI.Nodes;
using HK.ServerReact.Server.Services;

namespace HK.ServerReact.Server.Features.CounterStrikeFeatures;

public interface ICsEventSubscriber<T> where T : ICsEvent
{
    ValueTask OnEvent(T data);
}

public class KobeFeature : IFeature
{
    public void AddServices(IServiceCollection services) =>
         services.AddSingleton<ICsEventSubscriber<CEPlayerGotKill>, KobeCsEventSub>();

    public void MapEndpoints(WebApplication app) { }

    public class KobeCsEventSub(IBotConnectorServices bot) : ICsEventSubscriber<CEPlayerGotKill>
    {
        public async ValueTask OnEvent(CEPlayerGotKill data)
        {
            Weapon weapon = data.EventInfo.Weapon;
            if (weapon.Name.Equals(WeaponNames.HeGrenade, StringComparison.CurrentCultureIgnoreCase))
            {
                await bot.SendChatMessageAsync("Kobe!");
            }
        }
    }
}


public class ThatsHotFeature : IFeature
{
    public void AddServices(IServiceCollection services) =>
         services.AddSingleton<ICsEventSubscriber<CEPlayerDied>, ThatsHotEventSub>();

    public void MapEndpoints(WebApplication app) { }

    public class ThatsHotEventSub() : ICsEventSubscriber<CEPlayerDied>
    {
        public async ValueTask OnEvent(CEPlayerDied data)
        {

        }
    }
}


