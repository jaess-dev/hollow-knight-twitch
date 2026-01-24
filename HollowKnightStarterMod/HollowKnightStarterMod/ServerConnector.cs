using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HK.Domain;

namespace HollowKnightStarterMod
{
    public class ServerConnector(HttpClient value, Action<string> logError) : ICommunication
    {
        private readonly HttpClient _httpClient = value;
        private readonly Action<string> _logError = logError;

        public Task SendGeoEventAsync(int amountGained, int totalGeo)
        {
            // throw new System.NotImplementedException();
            return Task.CompletedTask;
        }

        public async Task SendYouDiedAsync()
        {
            await TransmitEventAsync(new DeathEvent());
        }

        private async Task TransmitEventAsync(IEvent @event)
        {
            var payload = new
            {
                @event
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            using StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(
                "https://localhost:54024/api/hk/event",
                content
            );
        }
    }
}