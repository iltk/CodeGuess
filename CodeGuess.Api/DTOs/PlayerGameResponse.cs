using CodeGuess.Api.Models;

namespace CodeGuess.Api.DTOs;

// This player's current view of the game, everything the player page needs to show it
public class PlayerGameResponse
{
    public GameState State { get; set; }

    // "Q 03 / 10": 1-based number of the current question, and how many the deck has
    public int QuestionNumber { get; set; }
    public int QuestionCount { get; set; }

    // The question hasn't started yet
    public bool WaitingForTimer { get; set; }

    // The question's time has run out
    public bool IsTimeUp { get; set; }

    public List<AnswerOptionResponse> AnswerOptions { get; set; } = new();

    // null if the player hasn't answered this question
    public int? AnswerId { get; set; }

    public bool? IsCorrect { get; set; }
}
