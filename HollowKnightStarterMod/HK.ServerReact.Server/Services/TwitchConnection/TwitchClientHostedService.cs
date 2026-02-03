using HK.ServerReact.Server.Services.TwitchConnection;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;


namespace twitch_con.TwClient;

public class TwitchClientHostedService : IHostedService
{
    private readonly ILogger<TwitchClientHostedService> _logger;
    private readonly ITwitchClient _client;
    private readonly string _botName;
    private readonly ITokenStore _tokenStore;
    private readonly ITwitchClientSubscriber[] _subs;

    public TwitchClientHostedService(
        [FromKeyedServices(DiKeys.BOT_NAME)] string botName,
        ITwitchClient client,
        ITokenStore tokenStore,
        IEnumerable<ITwitchClientSubscriber> subscribers,
        ILogger<TwitchClientHostedService> logger)
    {
        _logger = logger;
        _botName = botName;
        _client = client;
        _tokenStore = tokenStore;
        _subs = subscribers.ToArray();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (var sub in _subs)
            sub.Sub(_client);

        _tokenStore.OnChange += OnTokenStoreChange;
        await InitializeWithCred(_tokenStore);
    }

    private async void OnTokenStoreChange(ITokenStore ts)
    {
        try
        {
            await InitializeWithCred(ts);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error on reinitializing the client: {EXCEPTION}", ex);
        }
    }

    private async Task InitializeWithCred(ITokenStore ts)
    {
        TwitchToken[]? tokens = [.. await ts.LoadAsync() ?? []];
        ConnectionCredentials cred;
        switch (tokens?.FirstOrDefault(t => t.ExpiresAtUtc > DateTimeOffset.UtcNow))
        {
            case TwitchToken token:
                cred = new(_botName, $"oauth:{token.AccessToken}");
                _logger.LogInformation("Creating bot with credentials");
                break;
            default:
                cred = new();
                _logger.LogInformation("Creating bot without credentials");
                break;
        }

        if (_client.IsConnected is true)
        {
            await _client.DisconnectAsync();
        }

        _client.Initialize(cred);
        await _client.ConnectAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken) => await _client.DisconnectAsync();
}