using CodeGuess.Mvc.DTOs;

namespace CodeGuess.Mvc.Services;

public interface IDeckService
{
    Task<List<DeckResponse>> GetDecksAsync();
}
