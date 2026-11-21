using CodeGuess.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeGuess.Api.Data;

// Lookups shared by the game services
public static class GameQueryExtensions
{
    public static async Task<GameSession> GetSessionAsync(this AppDbContext db, string joinCode)
    {
        var session = await db.GameSessions
            .FirstOrDefaultAsync(s => s.JoinCode == joinCode);

        if (session == null)
            throw new KeyNotFoundException($"Game {joinCode} not found.");

        return session;
    }

    // The question the game is on right now
    public static async Task<Question> GetCurrentQuestionAsync(this AppDbContext db, GameSession session)
    {
        var question = await db.Questions
            .FirstOrDefaultAsync(q => q.DeckId == session.DeckId
                                   && q.Position == session.CurrentQuestionPosition);

        if (question == null)
            throw new KeyNotFoundException("The current question was not found.");

        return question;
    }
}
