namespace CodeGuess.Api.Services;

public interface IGameTimerService
{
    Task StartAsync(string joinCode, int seconds);
}
