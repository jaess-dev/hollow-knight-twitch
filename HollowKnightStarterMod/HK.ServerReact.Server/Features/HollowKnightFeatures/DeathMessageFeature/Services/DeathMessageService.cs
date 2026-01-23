using HK.Domain;
using HK.ServerReact.Server.Features.HollowKnightFeatures;
using HK.ServerReact.Server.Features.HollowKnightFeatures.DeathMessageFeature.Models;
using HK.ServerReact.Server.Services;

namespace HK.ServerReact.Server.Features.HollowKnightFeatures.DeathMessageFeature.Services;

public sealed class DeathMessageService(
    BotConnectorServices botConnector,
    DeathMessageContainer messages) : IHkSubscriber<DeathEvent>
{
    private readonly DeathMessageContainer _messages = messages;

    public async void OnReceived(DeathEvent @event)
    {
        var random = new Random();
        string message = _messages[random.Next(_messages.Length)];
        await botConnector.SendChatMessageAsync(message);
    }
}