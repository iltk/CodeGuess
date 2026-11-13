using CodeGuess.Mvc.DTOs;
using CodeGuess.Mvc.Models;

namespace CodeGuess.Mvc.Services;

public interface IGameService
{
    Task<GameResponse?> CreateGameAsync(int deckId);

    Task<bool> JoinGameAsync(JoinGameRequest request);

    Task<bool> StartGameAsync(string joinCode);

    Task<GameResponse?> GetGameAsync(string joinCode);

    Task<List<PlayerResponse>?> GetLeaderboardAsync(string joinCode);

    Task<GameState?> NextQuestionAsync(string joinCode);
}
