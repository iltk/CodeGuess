using CodeGuess.Mvc.DTOs;
using CodeGuess.Mvc.Models;

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

    public async Task<bool> StartGameAsync(string joinCode)
    {
        var response = await _http.PostAsync($"api/game/{joinCode}/start", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<GameResponse?> GetGameAsync(string joinCode)
    {
        var response = await _http.GetAsync($"api/game/{joinCode}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<GameResponse>();
    }
    
    
    //call api to get the results 
    // need player and its place like 1 , 2 ,3 ...
    //LeaderBoardResponse - list of playersDto 
    //where player dto will have name score and places soprted 
    public async Task<List<PlayerResponse>?> GetLeaderboardAsync(string joinCode)
    {
        var response = await _http.GetAsync($"api/game/{joinCode}/leaderboard");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<List<PlayerResponse>>();
    }

    public async Task<GameState?> NextQuestionAsync(string joinCode)
    {
        var response = await _http.PostAsync($"api/game/{joinCode}/next", null);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<GameState>();
    }
}
