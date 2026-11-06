using System.ComponentModel.DataAnnotations;

namespace CodeGuess.Api.Models;

public class Question
{
    public int Id { get; set; }
    public int DeckId { get; set; }
    
    [Required, StringLength(50)]
    public string Code { get; set; } = string.Empty;

    // Each question belongs to one deck
    public Deck Deck { get; set; } = null!;

    // One question has many answer options
    [Required]
    public ICollection<AnswerOption> AnswerOptions { get; set; }
        = new List<AnswerOption>();
}