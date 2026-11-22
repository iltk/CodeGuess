using CodeGuess.Api.Data;
using CodeGuess.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CodeGuess.Api.Services;

public class AnswerService : IAnswerService
{
    private readonly AppDbContext _db;

    public AnswerService(AppDbContext db)
    {
        _db = db;
    }

    
    public async Task<bool> SubmitAnswerAsync(string joinCode, int playerId, int answerId)
    {
        DateTime receivedAt = DateTime.UtcNow;

        var session = await _db.GetSessionAsync(joinCode);

        var player = await _db.Players
            .FirstOrDefaultAsync(p => p.Id == playerId && p.GameSessionId == session.Id);

        if (player == null)
            throw new KeyNotFoundException($"Player {playerId} not found in this game.");

        var question = await _db.GetCurrentQuestionAsync(session);

        var answer = await _db.AnswerOptions
            .FirstOrDefaultAsync(a => a.Id == answerId && a.QuestionId == question.Id);

        if (answer == null)
            throw new InvalidOperationException("This answer is not an option for the current question.");

        if (player.LastAnsweredPosition == session.CurrentQuestionPosition)
            throw new InvalidOperationException("You already answered this question.");

        // remember this question is answered, whether the answer is right or wrong
        player.LastAnsweredPosition = session.CurrentQuestionPosition;
        player.LastAnswerId = answerId;

        if (!answer.IsCorrect)
        {
            await _db.SaveChangesAsync();
            return false;
        }

        double responseTime = (double)((receivedAt - session.QuestionStartedAt)?.TotalSeconds ?? 0);

        player.Score += ScoreCalculator.Calculate(responseTime);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetCorrectAnswerIdAsync(string joinCode)
    {
        var session = await _db.GetSessionAsync(joinCode);
        var question = await _db.GetCurrentQuestionAsync(session);

        var correctAnswer = await _db.AnswerOptions
            .FirstOrDefaultAsync(a => a.QuestionId == question.Id && a.IsCorrect);

        if (correctAnswer == null)
            throw new KeyNotFoundException("This question has no correct answer.");

        return correctAnswer.Id;
    }
}
