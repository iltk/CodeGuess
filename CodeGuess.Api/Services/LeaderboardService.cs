using CodeGuess.Api.Data;
using CodeGuess.Api.DTOs;
using CodeGuess.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeGuess.Api.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly AppDbContext _db;

    public LeaderboardService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<PlayerResponse>> GetLeaderboardAsync(string joinCode)
    {
        var session = await _db.GetSessionAsync(joinCode);

        //get players and sort them by their score
        var players = await _db.Players
            .Where(p => p.GameSessionId == session.Id)
            .OrderByDescending(p => p.Score)
            .ToListAsync();

        var leaderboard = new List<PlayerResponse>();

        foreach (Player player in players)
        {
            leaderboard.Add(new PlayerResponse
            {
                Id = player.Id,
                Nickname = player.Nickname,
                GameSessionId = session.Id,
                Score = player.Score,
            });
        }

        return leaderboard;
    }

    public async Task<PlayerStatResponse> GetPlayerStat(string joinCode, int playerId)
    {
        int playerPlace = await GetPlayerPlaceAsync(joinCode, playerId);
        double playerScore = await GetPlayerScoreAsync(joinCode, playerId);

        return new PlayerStatResponse
        {
            Place = playerPlace,
            Score = playerScore,
        };
    }

    private async Task<int> GetPlayerPlaceAsync(string joinCode, int playerId)
    {
        var session = await _db.GetSessionAsync(joinCode);

        var players = await _db.Players
            .Where(p => p.GameSessionId == session.Id)
            .OrderByDescending(p => p.Score)
            .ToListAsync();

        if (players == null)
        {
            throw new KeyNotFoundException($"Player {playerId} not found in this game.");
        }

        int playerPosition = 1;

        foreach (Player player in players)
        {
            if (player.Id == playerId)
            {
                return playerPosition;
            }
            playerPosition++;
        }

        return 0;
    }

    private async Task<double> GetPlayerScoreAsync(string joinCode, int playerId)
    {
        var session = await _db.GetSessionAsync(joinCode);

        var player = await _db.Players
            .FirstOrDefaultAsync(p => p.GameSessionId == session.Id && p.Id == playerId);

        if (player == null)
        {
            throw new KeyNotFoundException($"Player {playerId} not found in this game.");
        }

        return player.Score;
    }
}
