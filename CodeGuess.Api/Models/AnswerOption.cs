using System.ComponentModel.DataAnnotations;

namespace CodeGuess.Api.Models;

public class AnswerOption
{
    public int Id { get; set; }
    
    public int QuestionId { get; set; }
    
    [Required, StringLength(50)]
    public string Text { get; set; } = string.Empty;
    
    [Required]
    public bool IsCorrect { get; set; }

    // each answer option belongs to one question
    [Required]
    public Question Question { get; set; } = null!;
}