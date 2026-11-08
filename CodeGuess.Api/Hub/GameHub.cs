namespace CodeGuess.Api.Hub;
using Microsoft.AspNetCore.SignalR;

public class GameHub: Hub
{
    // Browser calls this after connecting, to subscribe to one lobby's updates
    public async Task JoinRoom(string code)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, code);
    }

    
}