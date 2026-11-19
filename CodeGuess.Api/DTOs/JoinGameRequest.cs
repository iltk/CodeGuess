using System.ComponentModel.DataAnnotations;

namespace CodeGuess.Api.DTOs;

public class JoinGameRequest
{
    [Required]
    [StringLength(20)]
    public string Nickname { get; set; } = string.Empty;
}
