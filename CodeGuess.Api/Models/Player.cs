using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CodeGuess.Api.Models;

public class Player
{
    public int Id { get; set; }

    [Required, StringLength(20)]
    public string Nickname { get; set; } = string.Empty;

    public int GameSessionId { get; set; }

    public GameSession GameSession { get; set; } = null!;
    
    // position of the last question this player answered 
    public int? LastAnsweredPosition { get; set; }

    // the answer option this player picked for that question
    public int? LastAnswerId { get; set; }

    public double Score { get; set; } = 0;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
