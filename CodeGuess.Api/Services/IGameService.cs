using CodeGuess.Api.DTOs;

namespace CodeGuess.Api.Services;

public interface IGameService
{
    Task<GameResponse?> CreateGameAsync(int deckId);

    Task JoinGameAsync(string joinCode, JoinGameRequest request);

    Task StartGameAsync(string joinCode);

    Task<GameResponse> GetGameAsync(string joinCode);
    
    Task<List<PlayerResponse>> GetLeaderBoardAsync(string joinCode);
}
