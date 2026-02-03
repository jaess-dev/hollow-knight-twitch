using System.Text;
using HK.ServerReact.Server.Features.TwClient.Hubs;
using HK.ServerReact.Server.Services.TwitchConnection;
using HK.ServerReact.Server.Util;
using twitch_con.TwClient;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;


namespace HK.ServerReact.Server.Features.TwClient.Subs;




public class MisreadCounter(
    ILogger<MisreadCounter> logger,
    int sessionCounter = 0,
    [FromKeyedServices(MisreadCounter.MisreadCounterMessageTemplatesDiKey)] Dictionary<string, string[]>? messageTemplates = null) : IMessageHandler
{
    private readonly ILogger<MisreadCounter> _logger = logger;

    public const string MisreadCounterMessageTemplatesDiKey = "MisreadCounterMessageTemplatesDiKey";

    public readonly Dictionary<int, string[]> _templateMessages = InitTemplates(messageTemplates);
    private int _sessionCounter = sessionCounter;

    public bool HandlesMessage(ChatMessage message) => message.Message.Contains("!misread", StringComparison.CurrentCultureIgnoreCase);

    public async Task HandleMessageAsync(ITwitchClient client, ChatMessage message)
    {
        _sessionCounter += 1;

        var responseTemplate = _templateMessages[_templateMessages.Keys.Where(k => k < _sessionCounter).Max()];

        string response = new StringBuilder(responseTemplate.AsSpan().ChooseRandom())
            .Replace("{counter}", _sessionCounter.ToString())
            .ToString();

        await client.SendReplyAsync(message.Channel, message.Id, response);
    }

    public static Dictionary<int, string[]> InitTemplates(Dictionary<string, string[]>? templates)
    {
        Dictionary<int, string[]> dict = [];
        if (templates is null) return AddDefault(dict);

        foreach (var (k, v) in templates)
        {
            if (int.TryParse(k, out int key))
                dict[key] = v;
        }

        return dict.ContainsKey(0) ? dict : AddDefault(dict);

        static Dictionary<int, string[]> AddDefault(Dictionary<int, string[]> dict)
        {
            dict[0] = ["The {counter} message that was read wrong!"];
            return dict;
        }
    }
}

