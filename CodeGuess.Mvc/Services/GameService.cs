using CodeGuess.Mvc.DTOs;

namespace CodeGuess.Mvc.Services;


// Talks to the API's game endpoints
public class GameService : IGameService
{
    private readonly HttpClient _http;

    public GameService(HttpClient http)
    {
        _http = http;
    }

    public async Task<GameResponse?> CreateGameAsync(int deckId)
    {
        var response = await _http.PostAsJsonAsync("api/game", new { deckId });

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<GameResponse>();
    }

    public async Task<bool> JoinGameAsync(JoinGameRequest request)
    {
        var response = await _http.PostAsJsonAsync(
            $"api/game/{request.JoinCode}/join",
            new { request.Nickname });

        return response.IsSuccessStatusCode;
    }

    public async Task StartGameAsync(string joinCode)
    {
        //TODO: add error handling
        await _http.PostAsync($"api/game/{joinCode}/start", null);
    }

    public async Task<GameResponse?> GetGameAsync(string joinCode)
    {
        var response = await _http.GetAsync($"api/game/{joinCode}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<GameResponse>();
    }
}
