namespace CodeGuess.Api.Services;

public interface IAnswerService
{
    Task<bool> SubmitAnswerAsync(string joinCode, int playerId, int answerId);

    Task<int> GetCorrectAnswerIdAsync(string joinCode);
}
