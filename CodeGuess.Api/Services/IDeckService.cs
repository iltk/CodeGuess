using CodeGuess.Api.DTOs;
using CodeGuess.Api.Models;

namespace CodeGuess.Api.Services;

public interface IDeckService
{
    
    Task<List<Deck>> GetDecksAsync();
    

    
}