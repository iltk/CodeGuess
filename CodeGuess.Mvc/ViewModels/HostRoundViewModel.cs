namespace CodeGuess.Mvc.ViewModels;

public class HostRoundViewModel
{
    public string JoinCode { get; set; } = "";

    public int CurrentQuestionPosition { get; set; }

    public int TotalQuestions { get; set; }

    public string? Code { get; set; }

    public List<AnswerOptionViewModel> AnswerOptions { get; set; } = new();
}
