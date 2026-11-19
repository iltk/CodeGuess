using System.ComponentModel.DataAnnotations;

namespace CodeGuess.Api.Models;

public class GameSession
{
    public int Id { get; set; }

    public int DeckId { get; set; }

    [Required, StringLength(4)]
    public string JoinCode { get; set; } = string.Empty;

    public GameState State { get; set; } = GameState.Lobby;
    public int CurrentQuestionPosition { get; set; } = 0;

    public DateTime? QuestionStartedAt { get; set; }

    public Deck Deck { get; set; } = null!;

    public ICollection<Player> Players { get; set; } = new List<Player>();

}
