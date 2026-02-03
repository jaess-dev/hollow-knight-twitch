using TwitchLib.Client.Interfaces;

namespace HK.ServerReact.Server.Services.TwitchConnection;

/// <summary>
/// Service for wrapping the twitch client. 
/// </summary>
/// <param name="targetChannel">The channel which should be used as target for actions taking a channel as target.</param>
/// <param name="twitchClient">The twitch client to use for interaction with twitch.</param>
/// <param name="msDelay">The milliseconds the send message waits for sending a message if the provided client is currently not connected.</param>
public class TwitchService(
    [FromKeyedServices(DiKeys.TARGET_CHANNEL)] string targetChannel,
    ITwitchClient twitchClient,
    int msDelay=10_000) : IBotMessageSender
{
    private readonly ITwitchClient _client = twitchClient;
    private readonly string _targetChannel = targetChannel;
    private readonly int _msDelay = msDelay;

    public async Task SendChatMessageAsync(string message)
    {
        while (!_client.IsConnected)
        {
            await Task.Delay(_msDelay);
        }

        await _client.SendMessageAsync(_targetChannel, message);
    }
}