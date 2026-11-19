using CodeGuess.Mvc.DTOs;

namespace CodeGuess.Mvc.Services;

// gets deck data from the API
public class DeckService : IDeckService
{
    private readonly HttpClient _http;

    public DeckService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<DeckResponse>> GetDecksAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<DeckResponse>>("api/deck") ?? new();
        }
        catch (HttpRequestException)
        {
            return new(); // API is down or returned an error: show no decks
        }
    }
}
