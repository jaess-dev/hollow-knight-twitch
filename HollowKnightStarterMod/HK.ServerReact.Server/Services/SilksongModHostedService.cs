using HK.Domain;
using HK.ServerReact.Server.Features.HollowKnightFeatures;
using Microsoft.AspNetCore.SignalR.Client;
using TwitchLib.EventSub.Websockets.Client;

namespace HK.ServerReact.Server.Services;

public class SilksongModHostedService(
    IEnumerable<IHkSubscriber<DeathEvent>> hkSubscriber,
    ILogger<SilksongModHostedService> logger) : IHostedService
{
    private readonly ILogger<SilksongModHostedService> _logger = logger;

    private readonly IHkSubscriber<DeathEvent>[] _hkSubscriber = [.. hkSubscriber];

    private HubConnection? _hubConnection;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Silksong Mod Hosted Service is starting.");
        _hubConnection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:5000/hubs/deathcounter-hub")
                    .WithAutomaticReconnect()
                    .Build();

        // Subscribe to hub methods
        Dictionary<int, int>? deathCounters = null;
        _hubConnection.On<Dictionary<int, int>>("DeathCounterBatch",
            async (counters) =>
            {
                _logger.LogInformation("Received death counters: {@Counters}", counters);
                if (deathCounters is null)
                {
                    _logger.LogInformation("Initializing death counters.");
                    deathCounters = counters;
                    return;
                }

                if (deathCounters.SequenceEqual(counters))
                {
                    _logger.LogDebug("Death counters are the same as before, skipping.");
                    return;
                }

                deathCounters = counters;
                DeathEvent deathEvent = new();
                foreach (var subscriber in _hkSubscriber)
                    await subscriber.OnReceivedAsync(deathEvent);
            });

        // Handle connection events
        _hubConnection.Closed += async (error) =>
        {
            _logger.LogWarning("Connection closed. Error: {Error}", error?.Message);
            await Task.Delay(5000, cancellationToken);
        };

        try
        {
            await _hubConnection.StartAsync(cancellationToken);
            _logger.LogInformation("Connected to SignalR hub.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to SignalR hub.");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Silksong Mod Hosted Service is stopping.");

        if (_hubConnection is not null)
        {
            await _hubConnection.StopAsync(cancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}


public class DeathCounterEntry
{
    public int ProfileId { get; set; }
    public int DeathCounter { get; set; }

    public DeathCounterEntry(int profileId, int deathCounter)
    {
        ProfileId = profileId;
        DeathCounter = deathCounter;
    }
}
