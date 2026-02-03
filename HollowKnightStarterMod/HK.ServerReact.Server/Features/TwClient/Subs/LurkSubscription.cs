using System.Text;
using HK.ServerReact.Server.Util;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;

namespace HK.ServerReact.Server.Features.TwClient.Subs;

public class LurkSubscription(
    ILogger<LurkSubscription> logger,
    [FromKeyedServices(LurkSubscription.LurkSubscriptionTemplateMessagesDiKey)] string[]? messageTemplates = null) : IMessageHandler
{
    private readonly ILogger<LurkSubscription> _logger = logger;

    public const string LurkSubscriptionTemplateMessagesDiKey = "LurkSubscriptionTemplateMessagesDiKey";

    public readonly string[] _templateMessages = messageTemplates is { Length: > 0 } ? messageTemplates : ["{name} is now lurking!"];

    public bool HandlesMessage(ChatMessage message) => message.Message.Contains("!lurk", StringComparison.CurrentCultureIgnoreCase);

    public async Task HandleMessageAsync(ITwitchClient client, ChatMessage message)
    {
        string responseTemplate = new StringBuilder(_templateMessages.AsSpan().ChooseRandom())
            .Replace("{name}", message.DisplayName)
            .ToString();

        await client.SendMessageAsync(message.Channel, responseTemplate);
    }
}

