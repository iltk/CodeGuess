using CodeGuess.Mvc.DTOs;

namespace CodeGuess.Mvc.Services;

public interface IGameService
{
    Task<GameResponse?> CreateGameAsync(int deckId);

    Task<bool> JoinGameAsync(JoinGameRequest request);

    Task StartGameAsync(string joinCode);

    Task<GameResponse?> GetGameAsync(string joinCode);
}
