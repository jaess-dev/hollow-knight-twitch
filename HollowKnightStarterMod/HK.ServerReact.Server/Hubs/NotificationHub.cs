using Microsoft.AspNetCore.SignalR;

namespace HK.ServerReact.Server.Hubs;

public class NotificationsHub : Hub
{
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}