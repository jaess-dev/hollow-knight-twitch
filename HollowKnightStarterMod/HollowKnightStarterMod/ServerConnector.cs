using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HK.Domain;
using HollowKnightStarterMod.Domain.Model;

namespace HollowKnightStarterMod
{
    public class ServerConnector(HttpClient value, Action<string> logError) : ICommunication
    {
        private readonly HttpClient _httpClient = value;
        private readonly Action<string> _logError = logError;

        public Task SendGeoEventAsync(int amountGained, int totalGeo) => Task.CompletedTask;
        public Task SendYouDiedAsync() => TransmitEventAsync(new DeathEvent());
        public Task SendGrubSavedAsync(int grubCount) => TransmitEventAsync(new GrubSavedEvent(grubCount));
        public Task SendRespawnAsync(PlayerDataDto playerData) => TransmitEventAsync(new RespawnEvent(playerData));
        public Task SendDiedFromHazardAsync(HazardDeathDto hazardDeathDto) => TransmitEventAsync(new HazardDeathEvent(hazardDeathDto));


        private async Task TransmitEventAsync(IEvent @event)
        {
            var payload = new { @event };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            using StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(
                "https://localhost:54024/api/hk/event",
                content
            );
        }
    }
}