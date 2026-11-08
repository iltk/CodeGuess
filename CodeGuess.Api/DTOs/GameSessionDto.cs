namespace CodeGuess.Api.DTOs;


// What the API sends back about a game session (returned by POST api/game/{deckId})
public class GameSessionDto
{
    public int Id { get; set; }

    public string JoinCode { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public int CurrentQuestionIndex { get; set; }

    // The deck this game takes its questions from
    public int DeckId { get; set; }

    public DateTime CreatedAt { get; set; }
}