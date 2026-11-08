using CodeGuess.Mvc.Models;

namespace CodeGuess.Mvc.Services;

public interface IDeckService
{
    Task<List<Deck>> GetDecksAsync();
}
