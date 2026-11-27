namespace CodeGuess.Mvc.ViewModels;

public class LobbyViewModel
{
    public string JoinCode { get; set; } = "";

    public string DeckName { get; set; } = "";
    public int QuestionCount { get; set; }
    public int QuestionSeconds { get; set; }  
}