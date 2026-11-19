using CodeGuess.Api.DTOs;
using CodeGuess.Api.Hubs;
using CodeGuess.Api.Models;
using Microsoft.AspNetCore.SignalR;

namespace CodeGuess.Api.Services;

public class GameTimerService : IGameTimerService
{
    private readonly IHubContext<GameHub> _hub;
    private readonly IServiceScopeFactory _scopeFactory;
    
    public GameTimerService(IHubContext<GameHub> hub, IServiceScopeFactory scopeFactory)
    {
        _hub = hub;
        _scopeFactory = scopeFactory;
        
    }

    public void Start(string joinCode, int seconds)
    {
        RunAsync(joinCode, seconds);  
    }

    private async Task RunAsync(string joinCode, int seconds)
    {
        using (var scope = _scopeFactory.CreateScope())                                                                                                                            
        {                                        
            //get session and set startTime to now
                
            var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();                                                                                            
            await gameService.MarkQuestionStartedAsync(joinCode);   
            List<AnswerOptionResponse> answers = await gameService.GetAnswerOptionsAsync(joinCode);
            
            await _hub.Clients.Group(joinCode).SendAsync("QuestionStarted", new                                                                                                                
            {                                                                                                                                                                                  
                Options = answers,                                                                                                                                                             
                Seconds = seconds                                                                                                                                                              
            }); 
        } 
        await _hub.Clients.Group(joinCode).SendAsync("TimerStarted", seconds);
        
        await Task.Delay(seconds * 1000);
        
        await _hub.Clients.Group(joinCode).SendAsync("TimerStopped");
    }
}
