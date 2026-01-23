using System.Threading.Tasks;

namespace HK.Domain
{
    public interface ICommunication
    {
        Task SendYouDiedAsync();
        Task SendGeoEventAsync(int amountGained, int totalGeo);
    }
}