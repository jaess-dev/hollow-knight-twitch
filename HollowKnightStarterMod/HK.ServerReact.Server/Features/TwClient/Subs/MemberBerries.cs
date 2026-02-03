using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace HK.ServerReact.Server.Features.TwClient.Subs;

public sealed class MemberBerries : IMessageHandler
{
    public bool HandlesMessage(ChatMessage message) => message.Message.Contains("remember", StringComparison.CurrentCultureIgnoreCase);

    public Task HandleMessageAsync(ITwitchClient client, ChatMessage message)
        => client.SendReplyAsync(message.Channel, message.Id, "I member");
}