using CodeGuess.Api.DTOs;
using CodeGuess.Api.Models;

namespace CodeGuess.Api.Services;

public interface IGameService
{
    // Methods throw KeyNotFoundException (not found) or InvalidOperationException (not allowed);
    // GameController catches them and returns 404 / 400.

    Task<GameResponse> CreateGameAsync(int deckId);

    Task<int> JoinGameAsync(string joinCode, JoinGameRequest request);

    Task StartGameAsync(string joinCode);

    Task<GameResponse> GetGameAsync(string joinCode);
    
    Task<List<PlayerResponse>> GetLeaderboardAsync(string joinCode);

    Task<int> GetCorrectAnswerIdAsync(string joinCode);

    Task<GameState> AdvanceGameAsync(string joinCode);

    Task<bool> SubmitAnswerAsync(string joinCode, int playerId, int answerId);
}
