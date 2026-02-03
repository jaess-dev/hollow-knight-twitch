using System.Text;
using System.Text.Json;
using HK.Domain;
using HollowKnightStarterMod.Domain.Model;

namespace HK.ServerReact.Server.Services;

public interface IBotMessageSender
{
    Task SendChatMessageAsync(string message);
}

public interface IBotConnectorServices : IBotMessageSender
{
    Task SendDiedFromHazardAsync(HazardDeathDto hazardDeathDto);
    Task SendGeoEventAsync(int amountGained, int totalGeo);
    Task SendGrubSavedAsync(int grubCount);
    Task SendRespawnAsync();
    Task SendRespawnAsync(PlayerDataDto playerData);
    Task SendYouDiedAsync();
}


public class BotWsConnectorService(
    IWebSocketService webSocketService,
    ILogger<BotWsConnectorService> logger) : ICommunication, IBotConnectorServices
{
    private readonly ILogger<BotWsConnectorService> _logger = logger;
    private readonly IWebSocketService _ws = webSocketService;

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

    public Task SendDiedFromHazardAsync(HazardDeathDto hazardDeathDto)
    {
        return Task.CompletedTask;
    }

    public Task SendGrubSavedAsync(int grubCount)
    {
        return Task.CompletedTask;
    }
    public Task SendRespawnAsync()
    {
        return Task.CompletedTask;
    }
    public Task SendRespawnAsync(PlayerDataDto playerData)
    {
        return Task.CompletedTask;
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

    public Task SendChatMessageAsync(string message)
    {
        string id = "46753d56-a895-46a8-9a50-749ffd8e120e";
        string name = "WebEndpoint";

        return SendMessageAsync(id, name, new
        {
            msg = message
        });
    }

    private async Task SendMessageAsync(string id, string name, object args)
    {
        var payload = new
        {
            request = "DoAction",
            id = Guid.NewGuid(), // this is arbitrary. Just marks request response pairs
            action = new
            {
                id,
                name,
            },
            args
        };

        string json = JsonSerializer.Serialize(payload);
        await _ws.SendAsync(json);
    }
}

public class BotHttpConnectorService(
    IHttpClientFactory httpClientFactory,
    ILogger<BotHttpConnectorService> logger) : ICommunication, IBotConnectorServices
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<BotHttpConnectorService> _logger = logger;

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

    public Task SendDiedFromHazardAsync(HazardDeathDto hazardDeathDto)
    {
        return Task.CompletedTask;
    }

    public Task SendGrubSavedAsync(int grubCount)
    {
        return Task.CompletedTask;
    }
    public Task SendRespawnAsync()
    {
        return Task.CompletedTask;
    }
    public Task SendRespawnAsync(PlayerDataDto playerData)
    {
        return Task.CompletedTask;
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

    public Task SendChatMessageAsync(string message)
    {
        string id = "46753d56-a895-46a8-9a50-749ffd8e120e";
        string name = "WebEndpoint";

        return SendMessageAsync(id, name, new
        {
            msg = message
        });
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