using CodeGuess.Api.DTOs;
using CodeGuess.Api.Models;

namespace CodeGuess.Api.Services;

public interface IGameService
{
    Task<GameSessionDto?> CreateGameAsync(int deckId);


}