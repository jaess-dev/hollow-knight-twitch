using System.Collections.Generic;
using HollowKnightStarterMod.Domain.Model;

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

    public class GrubSavedEvent(int grubCount) : IEvent
    {
        public string ClassName => nameof(GrubSavedEvent);

        public int GrubCount { get; set; } = grubCount;
    }


    public class RespawnEvent(PlayerDataDto playerData) : IEvent
    {
        public string ClassName => nameof(RespawnEvent);
        public PlayerDataDto PlayerData { get; set; } = playerData;

    }

    public class HazardDeathEvent(HazardDeathDto hazardDeathDto) : IEvent
    {
        public string ClassName => nameof(HazardDeathEvent);
        public HazardDeathDto HazardDeathDto { get; set; } = hazardDeathDto;
    }
}
