namespace CodeGuess.Api.Models;


public class GameSession
{
    public int Id { get; set; }

    public int DeckId { get; set; }

    public string JoinCode { get; set; } = string.Empty;

    public GameState State { get; set; } = GameState.Lobby;
    public int CurrentQuestionPosition { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Deck Deck { get; set; } = null!;
    
    public ICollection<Player> Players { get; set; } = new List<Player>();

}