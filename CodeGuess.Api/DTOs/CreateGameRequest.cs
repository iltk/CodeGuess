using System.ComponentModel.DataAnnotations;

namespace CodeGuess.Api.DTOs;

public class CreateGameRequest
{
    [Range(1, int.MaxValue)]
    public int DeckId { get; set; }
}
