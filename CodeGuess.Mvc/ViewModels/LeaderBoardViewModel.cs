namespace CodeGuess.Mvc.ViewModels;

public class LeaderBoardViewModel
{
    public string JoinCode { get; set; } = "";

    public bool IsFinished { get; set; }

    public List<LeaderBoardPlayerViewModel> Players { get; set; } = new();
}
