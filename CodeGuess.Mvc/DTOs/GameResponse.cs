using CodeGuess.Mvc.Models;

namespace CodeGuess.Mvc.DTOs;

// Used to read the API's game JSON (POST api/game, GET api/game/{joinCode})
public class GameResponse
{
    public int Id { get; set; }

    public string JoinCode { get; set; } = string.Empty;

    public GameState State { get; set; } = GameState.Lobby;

    // The deck this game takes its questions from
    public int DeckId { get; set; }

    // When the current question started (null until the game starts)
    public DateTime? QuestionStartedAt { get; set; }

    // 0-based position of the current question in the deck
    public int CurrentQuestionPosition { get; set; }

    // How many questions the deck has
    public int TotalQuestions { get; set; }

    public QuestionResponse? Question { get; set; }

    public List<AnswerOptionResponse>? AnswerOptions { get; set; }
}
