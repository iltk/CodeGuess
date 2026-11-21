using CodeGuess.Api.Models;

namespace CodeGuess.Api.DTOs;

// Everything the player page needs to rebuild itself after a reload
public class PlayerRestoreResponse
{
    // Question while time is left, CorrectAnswer once time is up
    public GameState State { get; set; }

    // The question hasn't started yet
    public bool WaitingForTimer { get; set; }

    public List<AnswerOptionResponse> AnswerOptions { get; set; } = new();

    // null if the player hasn't answered this question
    public int? AnswerId { get; set; }

    public bool? IsCorrect { get; set; }
}
