using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CodeGuess.Api.Models;

public class Question
{
    public int Id { get; set; }
    public int DeckId { get; set; }

    public int Position { get; set; }

    [Required, StringLength(1000)]
    public string Code { get; set; } = string.Empty;

    // Each question belongs to one deck
    public Deck Deck { get; set; } = null!;

    // One question has many answer options
    public ICollection<AnswerOption> AnswerOptions { get; set; }
        = new List<AnswerOption>();
}
