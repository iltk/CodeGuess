namespace CodeGuess.Mvc.DTOs;

//  used to read the API's JSON response from POST api/game/{deckId}
public class GameSessionDto
{
    public int Id { get; set; }

    public string JoinCode { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public int CurrentQuestionIndex { get; set; }

    public int DeckId { get; set; }

    public DateTime CreatedAt { get; set; }
}