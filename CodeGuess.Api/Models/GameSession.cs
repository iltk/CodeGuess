namespace CodeGuess.Api.Models;


public class GameSession
{
    public int Id { get; set; }

    public int DeckId { get; set; }

    public string JoinCode { get; set; } = string.Empty;

    public string State { get; set; } = "Lobby";

    public int CurrentQuestionIndex { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Deck Deck { get; set; } = null!;
}