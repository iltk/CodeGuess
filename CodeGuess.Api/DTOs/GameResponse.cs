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

    // 0-based position of the current question in the deck
    public int CurrentQuestionPosition { get; set; }

    // How many questions the deck has
    public int TotalQuestions { get; set; }

    public QuestionResponse? Question { get; set; }

    public List<AnswerOptionResponse>? AnswerOptions { get; set; }
}
