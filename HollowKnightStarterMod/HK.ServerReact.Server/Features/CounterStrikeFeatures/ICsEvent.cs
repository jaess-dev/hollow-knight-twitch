using CounterStrike2GSI.EventMessages;

namespace HK.ServerReact.Server.Features.CounterStrikeFeatures;

public interface ICsEvent
{
    public string EventName => GetType().Name;
}


public abstract class CsBaseEvent<T>(T eventInfo) : ICsEvent
{
    public T EventInfo { get; set; } = eventInfo;
}

public record CEBombExploded : ICsEvent;

public class CEPlayerGotKill(PlayerGotKill eventInfo)
    : CsBaseEvent<PlayerGotKill>(eventInfo);

public class CEPlayerDied(PlayerDied eventInfo)
    : CsBaseEvent<PlayerDied>(eventInfo);

public class CEKillFeed(KillFeed eventInfo)
    : CsBaseEvent<KillFeed>(eventInfo);

public class CERoundStarted(RoundStarted eventInfo)
    : CsBaseEvent<RoundStarted>(eventInfo);

public class CERoundConcluded(RoundConcluded eventInfo)
    : CsBaseEvent<RoundConcluded>(eventInfo);

public class CEBombStateUpdated(BombStateUpdated eventInfo)
    : CsBaseEvent<BombStateUpdated>(eventInfo);

public class CEPlayerScoreChanged(PlayerScoreChanged eventInfo)
    : CsBaseEvent<PlayerScoreChanged>(eventInfo);