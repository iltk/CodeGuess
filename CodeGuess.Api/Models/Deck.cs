using System.ComponentModel.DataAnnotations;

namespace CodeGuess.Api.Models;

public class Deck
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    // One deck has many questions
    public ICollection<Question> Questions { get; set; } = new List<Question>();

}
