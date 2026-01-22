using System.Text;
using System.Text.Json;
using HK.Domain;

namespace HollowKnightStarterMod;

public class BotConnector(
    IHttpClientFactory httpClientFactory,
    ILogger<BotConnector> logger) : ICommunication
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<BotConnector> _logger = logger;

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
            _logger.LogError("Failed to send webhook: {EXCEPTION}", ex);
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
            _logger.LogError("Failed to send webhook: {EXCEPTION}", ex);
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

        string json = JsonSerializer.Serialize(payload);
        using StringContent content = new(json, Encoding.UTF8, "application/json");
        var client = _httpClientFactory.CreateClient("Streamer.bot.HttpClient");
        HttpResponseMessage response = await client.PostAsync(
            "http://127.0.0.1:7474/DoAction",
            content
        );
    }
}