using System.Net.WebSockets;
using System.Text;

namespace HK.ServerReact.Server.Services;

public interface IWebSocketService
{
    Task SendAsync(string message, CancellationToken ct = default);
    bool IsConnected { get; }
}


public sealed class WebSocketService(Uri uri, ILogger<WebSocketService> logger) : IWebSocketService, IAsyncDisposable
{
    private readonly Uri _uri = uri;
    private ClientWebSocket? _socket;

    public bool IsConnected => _socket?.State == WebSocketState.Open;

    internal async Task ConnectAsync(CancellationToken ct = default)
    {
        if (IsConnected)
            return;

        _socket?.Dispose();
        _socket = new ClientWebSocket();

        await _socket.ConnectAsync(_uri, ct);
        _ = Task.Run(() => ReceiveLoop(ct), ct);
    }

    public async Task SendAsync(string message, CancellationToken ct = default)
    {
        await ConnectAsync(ct);

        if (!IsConnected)
            throw new InvalidOperationException("WebSocket not connected.");

        var bytes = Encoding.UTF8.GetBytes(message);
        for (int i = 0; i < 3; i++)
        {
            try
            {
                await _socket!.SendAsync(bytes, WebSocketMessageType.Text, true, ct);
                break;
            }
            catch
            {
                await DisposeAsync();
                await ConnectAsync(ct);
            }
        }
    }

    private async Task ReceiveLoop(CancellationToken ct)
    {
        var buffer = new byte[4096];

        while (!ct.IsCancellationRequested && IsConnected)
        {
            var result = await _socket!.ReceiveAsync(buffer, ct);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await DisposeAsync();
                break;
            }

            logger.LogInformation("Message received {WS_MESSAGE}", Encoding.UTF8.GetString(buffer, 0, result.Count));
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (IsConnected)
        {
            await _socket!.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Shutdown",
                CancellationToken.None);
        }

        _socket?.Dispose();
    }
}
