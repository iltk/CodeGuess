using CodeGuess.Mvc.DTOs;

namespace CodeGuess.Mvc.Services;

public interface IGameService
{
    Task<GameSessionDto?> CreateGameAsync(int deckId);
    
    Task<bool> JoinGameAsync(JoinGameDto dto);                                                                                                                                         

}
