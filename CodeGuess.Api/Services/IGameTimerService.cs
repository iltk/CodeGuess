namespace CodeGuess.Api.Services;

public interface IGameTimerService
{
    void Start(string joinCode, int seconds);
}
