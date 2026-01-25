using CounterStrike2GSI;
using CounterStrike2GSI.EventMessages;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HK.ServerReact.Server.Features.CounterStrikeFeatures;


public sealed class CsEventDistributer : IFeature
{
    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton<CsHubService>();
        services.AddSingleton<CsEventListener>();
        services.AddHostedService<CsListenerRunner>();
    }


    public void MapEndpoints(WebApplication app)
    {
        app.MapHub<CsHub>("/hubs/cs");
        app.MapGet("/api/cs/game-state", (CsEventListener listener, ILogger<CsEventDistributer> logger) =>
        {
            logger.LogInformation("huh");
            logger.LogInformation("huh");
            logger.LogInformation("huh");
            logger.LogInformation("huh");

            return TypedResults.Ok("");
        });
    }
}

public sealed class CsListenerRunner(
    CsEventListener listener, ILogger<CsListenerRunner> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("CS listener runner started!");
        listener.Start();
        logger.LogInformation("CS listener runner start finished!");
        return Task.CompletedTask;
    }
}


public sealed class CsEventListener : IDisposable
{
    private readonly CsHubService _hub;
    private readonly IServiceProvider _provider;
    private readonly GameStateListener _gsl;
    private readonly ILogger<CsEventListener> _logger;

    public GameState GameState => _gsl.CurrentGameState;

    public CsEventListener(
        CsHubService hubService,
        IServiceProvider provider,
        ILogger<CsEventListener> logger,
        int? port = null)
    {
        _hub = hubService;
        _provider = provider;
        _logger = logger;
        _gsl = new(port ?? 4000);

        _gsl.BombExploded += async (x) => await SendEvent(new CEBombExploded());
        _gsl.PlayerGotKill += async (e) => await SendEvent(new CEPlayerGotKill(e));
        _gsl.PlayerDied += async (e) => await SendEvent(new CEPlayerDied(e));
        _gsl.KillFeed += async (e) => await SendEvent(new CEKillFeed(e));
        _gsl.RoundStarted += async (e) => await SendEvent(new CERoundStarted(e));
        _gsl.RoundConcluded += async (e) => await SendEvent(new CERoundConcluded(e));
        _gsl.BombStateUpdated += async (e) => await SendEvent(new CEBombStateUpdated(e));
        _gsl.PlayerScoreChanged += async (e) => await SendEvent(new CEPlayerScoreChanged(e));
    }

    public void Start() => _gsl.Start();
    public void Dispose() => _gsl.Stop();

    private async Task SendEvent<T>(T csEvent) where T : ICsEvent
    {
        var subscriber = _provider.GetRequiredService<IEnumerable<ICsEventSubscriber<T>>>();
        _logger.LogInformation("{EVENT}", csEvent);
        if (csEvent is CsBaseEvent<T> baseEvent)
        {
            _logger.LogInformation("{EVENT_DATA}", baseEvent.EventInfo);
        }
        await Task.WhenAll(
            subscriber.Select(s => s.OnEvent(csEvent).AsTask())
                .Concat([_hub.SendEvent<T>(csEvent)]));
    }
}