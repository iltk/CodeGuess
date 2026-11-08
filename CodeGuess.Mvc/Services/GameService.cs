using CodeGuess.Mvc.DTOs;

namespace CodeGuess.Mvc.Services;


public class GameService : IGameService
{
    private readonly HttpClient _http;

    public GameService(HttpClient http)
    {
        _http = http;
    }

    public async Task<GameSessionDto?> CreateGameAsync(int deckId)
    {
        var response = await _http.PostAsync($"api/game/{deckId}", null);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<GameSessionDto>();
    }
    
    public async Task<bool> JoinGameAsync(JoinGameDto dto)                                                                                                                             
    {                                                                                                                                                                                  
        var response = await _http.PostAsJsonAsync($"api/game/{dto.Code}/join", dto);                                                                                               
        return response.IsSuccessStatusCode;                                                                                                                                           
    }  
}
