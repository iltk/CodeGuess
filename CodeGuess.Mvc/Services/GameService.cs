using CodeGuess.Mvc.DTOs;
using CodeGuess.Mvc.Models;

namespace CodeGuess.Mvc.Services;

public class GameService : IGameService
{
    private readonly HttpClient _http;

    public GameService(HttpClient http)
    {
        _http = http;
    }

    public async Task<GameResponse?> CreateGameAsync(int deckId)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/game", new { deckId });

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<GameResponse>();
        }
        catch (HttpRequestException)
        {
            return null; 
        }
    }

    public async Task<int?> JoinGameAsync(JoinGameRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(
                $"api/game/{request.JoinCode}/join",
                new { request.Nickname });

            if (!response.IsSuccessStatusCode)
                return null;

           
            return await response.Content.ReadFromJsonAsync<int>();
        }
        catch (HttpRequestException)
        {
            return null; 
        }
    }

    public async Task<bool> StartGameAsync(string joinCode)
    {
        try
        {
            var response = await _http.PostAsync($"api/game/{joinCode}/start", null);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false; 
        }
    }

    public async Task<GameResponse?> GetGameAsync(string joinCode)
    {
        try
        {
            var response = await _http.GetAsync($"api/game/{joinCode}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<GameResponse>();
        }
        catch (HttpRequestException)
        {
            return null; 
        }
    }


    //call api to get the results
    // need player and its place like 1 , 2 ,3 ...
    //LeaderBoardResponse - list of playersDto
    //where player dto will have name score and places soprted
    public async Task<List<PlayerResponse>?> GetLeaderboardAsync(string joinCode)
    {
        try
        {
            var response = await _http.GetAsync($"api/game/{joinCode}/leaderboard");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<List<PlayerResponse>>();
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<GameState?> AdvanceGameAsync(string joinCode)
    {
        try
        {
            var response = await _http.PostAsync($"api/game/{joinCode}/advance", null);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<GameState>();
        }
        catch (HttpRequestException)
        {
            return null; 
        }
    }


}
