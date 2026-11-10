using CodeGuess.Api.Models;

namespace CodeGuess.Api.DTOs;


// What the API sends back about a game (POST api/game, GET api/game/{joinCode})
public class GameResponse
{
    public int Id { get; set; }

    public string JoinCode { get; set; } = string.Empty;

    public GameState State { get; set; } = GameState.Lobby;

    // The deck this game takes its questions from
    public int DeckId { get; set; }

    public DateTime CreatedAt { get; set; }

    public QuestionResponse? Question { get; set; }

    public List<AnswerOptionResponse>? AnswerOptions { get; set; }
}
