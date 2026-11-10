namespace CodeGuess.Mvc.DTOs;

public class QuestionResponse
{
    public int Id { get; set; }

    public int Position { get; set; }

    public string Code { get; set; } = string.Empty;
}
