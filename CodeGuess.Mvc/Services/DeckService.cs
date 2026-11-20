using CodeGuess.Mvc.DTOs;

namespace CodeGuess.Mvc.Services;

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
            return new(); 
        }
    }
}
