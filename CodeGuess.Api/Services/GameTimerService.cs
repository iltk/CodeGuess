using CodeGuess.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CodeGuess.Api.Services;

public class GameTimerService : IGameTimerService
{
    private readonly IHubContext<GameHub> _hub;
    private readonly IGameService _gameService;

    public GameTimerService(IHubContext<GameHub> hub, IGameService gameService)
    {
        _hub = hub;
        _gameService = gameService;
    }

    public async Task StartAsync(string joinCode, int seconds)
    {
        await _gameService.MarkQuestionStartedAsync(joinCode);

        await _hub.Clients.Group(joinCode).SendAsync("QuestionStarted");
        await _hub.Clients.Group(joinCode).SendAsync("TimerStarted", seconds);

        CountdownAsync(joinCode, seconds);
    }

    private async Task CountdownAsync(string joinCode, int seconds)
    {
        await Task.Delay(seconds * 1000);
        await _hub.Clients.Group(joinCode).SendAsync("TimerStopped");
    }
}
