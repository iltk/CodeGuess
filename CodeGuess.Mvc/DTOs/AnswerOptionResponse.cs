namespace CodeGuess.Mvc.DTOs;

// Answer option as the API sends it (no IsCorrect, so players can't see the answer)
public class AnswerOptionResponse
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;
}
