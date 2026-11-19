using CodeGuess.Api.Services;

namespace CodeGuess.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

public class GameHub: Hub
{
    
    private readonly IGameTimerService _timer;
    public GameHub(IGameTimerService timer) => _timer = timer;
    
    // clients calls this after connecting
    public async Task JoinGame(string joinCode)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, joinCode);
    }
    
    // Host calls this once when its Question page is connected                                                                                                                         
    public void StartTimer(string joinCode)                                                                                                                                        
    {                                                                                                                                                                              
        _timer.Start(joinCode, 10);                                                                          
    } 
}
