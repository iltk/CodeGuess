namespace CodeGuess.Mvc.Models;

using System.ComponentModel.DataAnnotations;


public class Deck
{
    public int Id { get; set; }
    
    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    
    
}