namespace HK.ServerReact.Server.Features.TwClient.Hubs;

public interface ITwitchHubEvent
{
    string ClassName => GetType().Name;
}