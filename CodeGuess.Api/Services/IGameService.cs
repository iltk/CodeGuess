using CodeGuess.Api.DTOs;
using CodeGuess.Api.Models;

namespace CodeGuess.Api.Services;

public interface IGameService
{


    Task<GameResponse?> CreateGameAsync(int deckId);

    Task<int> JoinGameAsync(string joinCode, JoinGameRequest request);

    Task StartGameAsync(string joinCode);

    Task<GameResponse> GetGameAsync(string joinCode);

    Task<GameState> AdvanceGameAsync(string joinCode);

    Task MarkQuestionStartedAsync(string joinCode);

}
