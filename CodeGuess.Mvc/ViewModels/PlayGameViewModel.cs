namespace CodeGuess.Mvc.ViewModels;

// Data for player Game page 
public class PlayGameViewModel
{
    public string JoinCode { get; set; } = "";

    public int PlayerId { get; set; }

    public string Nickname { get; set; } = "";

    public string DeckName { get; set; } = "";

    public int QuestionCount { get; set; }
}
