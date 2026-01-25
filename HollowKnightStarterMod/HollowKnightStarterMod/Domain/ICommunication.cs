using System.Collections.Generic;
using System.Threading.Tasks;
using HollowKnightStarterMod.Domain.Model;

namespace HK.Domain
{
    public interface ICommunication
    {
        Task SendYouDiedAsync();
        Task SendGeoEventAsync(int amountGained, int totalGeo);
        Task SendGrubSavedAsync(int grubCount);
        Task SendRespawnAsync(PlayerDataDto playerData);
        Task SendDiedFromHazardAsync(HazardDeathDto hazardDeathDto);
    }
}