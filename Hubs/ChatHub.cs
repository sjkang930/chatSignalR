using Microsoft.AspNetCore.SignalR;
namespace Chat.Hubs;

public class ChatHub : Hub
{
 public async Task SendMessage(string message)
    {
        Console.WriteLine($"Received message: {message}");
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
    public async Task AddToGroup(string groupName)
  {
    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

    await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
  }

  public async Task RemoveFromGroup(string groupName)
  {
    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

    await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
  }
  public override Task OnConnectedAsync()
  {
    Console.WriteLine("A Client Connected: " + Context.ConnectionId);
    return base.OnConnectedAsync();
  }

  public override Task OnDisconnectedAsync(Exception exception)
  {
    Console.WriteLine("A client disconnected: " + Context.ConnectionId);
    return base.OnDisconnectedAsync(exception);
  }

}
