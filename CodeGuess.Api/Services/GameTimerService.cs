using CodeGuess.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CodeGuess.Api.Services;

public class GameTimerService : IGameTimerService
{
    private readonly IHubContext<GameHub> _hub;

    public GameTimerService(IHubContext<GameHub> hub)
    {
        _hub = hub;
    }

    public void Start(string joinCode, int seconds)
    {
        RunAsync(joinCode, seconds);  
    }

    private async Task RunAsync(string joinCode, int seconds)
    {
        await _hub.Clients.Group(joinCode).SendAsync("TimerStarted", seconds);
        await Task.Delay(seconds * 1000);
        await _hub.Clients.Group(joinCode).SendAsync("TimerStopped");
    }
}
