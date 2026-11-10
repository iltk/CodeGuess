namespace CodeGuess.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

public class GameHub: Hub
{
    // Browser calls this after connecting, to get this game's updates
    public async Task JoinGame(string joinCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, joinCode);
    }
    
}