using CodeGuess.Api.DTOs;

namespace CodeGuess.Api.Services;

public interface ILeaderboardService
{
    Task<List<PlayerResponse>> GetLeaderboardAsync(string joinCode);

    Task<PlayerStatResponse> GetPlayerStat(string joinCode, int playerId);
}
