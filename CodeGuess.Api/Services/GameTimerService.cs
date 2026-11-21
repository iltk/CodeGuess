using CodeGuess.Api.DTOs;
using CodeGuess.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CodeGuess.Api.Services;

public class GameTimerService : IGameTimerService
{
    private readonly IHubContext<GameHub> _hub;
    private readonly IGameService _gameService;
    private readonly IAnswerService _answerService;

    public GameTimerService(IHubContext<GameHub> hub, IGameService gameService, IAnswerService answerService)
    {
        _hub = hub;
        _gameService = gameService;
        _answerService = answerService;
    }

    public async Task StartAsync(string joinCode, int seconds)
    {
        
        await _gameService.MarkQuestionStartedAsync(joinCode);
        List<AnswerOptionResponse> answers = await _answerService.GetAnswerOptionsAsync(joinCode);

        await _hub.Clients.Group(joinCode).SendAsync("QuestionStarted", new
        {
            Options = answers,
            Seconds = seconds
        });
        await _hub.Clients.Group(joinCode).SendAsync("TimerStarted", seconds);

        CountdownAsync(joinCode, seconds);
    }

    private async Task CountdownAsync(string joinCode, int seconds)
    {
        await Task.Delay(seconds * 1000);
        await _hub.Clients.Group(joinCode).SendAsync("TimerStopped");
    }
}
