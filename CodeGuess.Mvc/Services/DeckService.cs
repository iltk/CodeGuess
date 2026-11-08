using CodeGuess.Mvc.Models;

namespace CodeGuess.Mvc.Services;

// gets deck data from the API
public class DeckService : IDeckService
{
    private readonly HttpClient _http;

    public DeckService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Deck>> GetDecksAsync()
    {
        return await _http.GetFromJsonAsync<List<Deck>>("api/deck") ?? new();
    }
}
