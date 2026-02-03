using HK.ServerReact.Server.Features.TwClient.Hubs;
using HK.ServerReact.Server.Services.TwitchConnection;
using twitch_con.TwClient;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;


namespace HK.ServerReact.Server.Features.TwClient.Subs;

public interface IMessageHandler
{
    bool HandlesMessage(ChatMessage message);
    Task HandleMessageAsync(ITwitchClient client, ChatMessage message);
}

public class BaseSubscriptions(
    [FromKeyedServices(DiKeys.TARGET_CHANNEL)] string targetChannel,
    IEnumerable<IMessageHandler> messageHandlers,
    TwitchHubService twitchHub,
    ILogger<BaseSubscriptions> logger) : ITwitchClientSubscriber
{
    private readonly ILogger<BaseSubscriptions> _logger = logger;

    private readonly string _targetChannel = targetChannel;
    private readonly IMessageHandler[] _messageHandlers = messageHandlers.ToArray();
    private readonly TwitchHubService _twitchHub = twitchHub;
    private ITwitchClient _client = null!;


    public void Sub(ITwitchClient client)
    {
        _client = client;

        _client.OnConnected += Client_OnConnected;
        _client.OnDisconnected += Client_OnDisconnected;
        _client.OnJoinedChannel += Client_OnJoinedChannel;
        _client.OnMessageReceived += Client_OnMessageReceived;

        _client.OnError += async (sender, error) => { _logger.LogError("Error: {ERROR}", error); };
        _client.OnChannelStateChanged += async (sender, args) => { };
    }

    ITwitchClient Client(object? sender) => sender as ITwitchClient ?? _client;

    async Task Client_OnDisconnected(object? sender, OnDisconnectedArgs e)
    {
        _logger.LogInformation("The bot was disconnected");
    }

    async Task Client_OnConnected(object? sender, OnConnectedEventArgs e)
    {
        _logger.LogInformation("The bot was connected and will be joining the target channel: {TARGET_CHANNEL}", _targetChannel);
        await Client(sender).JoinChannelAsync(_targetChannel);
    }

    async Task Client_OnJoinedChannel(object? sender, OnJoinedChannelArgs e)
    {
        _logger.LogInformation($"Connected to {e.Channel}. Sending welcome message next.");
        await Client(sender).SendMessageAsync(e.Channel, $"Hey guys! I am a bot connected via TwitchLib! {DateTimeOffset.UtcNow}");
    }

    async Task Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        ChatMessage message = e.ChatMessage;
        _logger.LogInformation(
            "{USERNAME}#{CHANNEL}: {MESSAGE}",
            message.Username,
            message.Channel,
            message.Message);

        if (e.ChatMessage.IsMe)
        {
            _logger.LogInformation("Ignoring the message as we wrote it");
            return;
        }

        var client = Client(sender);
        await Task.WhenAll(
            _messageHandlers
                .Where(mh => mh.HandlesMessage(message))
                .Select(mh => mh.HandleMessageAsync(client, message))
                .Concat([_twitchHub.SendEvent(new TwitchMessageEvent(message))]));
    }
}

public record TwitchMessageEvent(ChatMessage ChatMessage) : ITwitchHubEvent;
