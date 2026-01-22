using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HollowKnightStarterMod;

public class BotConnector(
    HttpClient httpClient,
    Action<string> logError)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly Action<string> LogError = logError;

    public async Task SendYouDiedAsync()
    {
        try
        {
            var id = "2ecc3d2d-f835-4e82-b035-60159743fd2a";
            var name = "YouDied";
            await SendMessageAsync(id, name, new
            {
            });
        }
        catch (Exception ex)
        {
            LogError($"Failed to send webhook: {ex}");
        }
    }

    public async Task SendGeoEventAsync(int amountGained, int totalGeo)
    {
        try
        {
            var id = "63810722-42a8-48a7-b466-65b5a137c7a1";
            var name = "GrubSaved";
            await SendMessageAsync(id, name, new
            {
                customArg = "customValue",
                eventType = "geo_collected",
                amount = amountGained,
                total = totalGeo,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
        }
        catch (Exception ex)
        {
            LogError($"Failed to send webhook: {ex}");
        }
    }


    private async Task SendMessageAsync(string id, string name, object args)
    {

        var payload = new
        {
            action = new
            {
                id,
                name,
            },
            args
        };

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
        // string json = System.Text.Json.JsonSerializer.Serialize(payload);
        using StringContent content = new(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PostAsync(
            "http://127.0.0.1:7474/DoAction",
            content
        );
    }
}