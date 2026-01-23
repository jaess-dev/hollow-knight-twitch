namespace HK.Domain
{
    public interface IEvent
    {
        string ClassName { get; }
    }

    public class DeathEvent : IEvent
    {
        public string ClassName => nameof(DeathEvent);
    }
}
