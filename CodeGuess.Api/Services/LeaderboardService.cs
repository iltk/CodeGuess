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
        var session = await _db.GetSessionAsync(joinCode);
        var player = await GetPlayerAsync(session.Id, playerId);
        int playerPlace = await GetPlayerPlaceAsync(joinCode, playerId);

        // LastRoundPoints belongs to the last question they answered; if that isn't
        // the current one, they skipped it, so they got nothing this round
        bool answeredThisRound = player.LastAnsweredPosition == session.CurrentQuestionPosition;

        return new PlayerStatResponse
        {
            Place = playerPlace,
            Score = player.Score,
            RoundPoints = answeredThisRound ? player.LastRoundPoints : 0,
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

        int playerPlace = 1;

        foreach (Player player in players)
        {
            if (player.Id == playerId)
            {
                return playerPlace;
            }
            playerPlace++;
        }

        return 0;
    }

    private async Task<Player> GetPlayerAsync(int sessionId, int playerId)
    {
        var player = await _db.Players
            .FirstOrDefaultAsync(p => p.GameSessionId == sessionId && p.Id == playerId);

        if (player == null)
        {
            throw new KeyNotFoundException($"Player {playerId} not found in this game.");
        }

        return player;
    }
}
