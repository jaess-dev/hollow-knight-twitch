using HK.Domain;

namespace HK.ServerReact.Server.Features.HollowKnightFeatures;

public interface IHkSubscriber<TEvent> : ITypedFeature
    where TEvent : IEvent
{
    void OnReceived(TEvent @event);
}
