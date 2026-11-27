namespace CodeGuess.Api.DTOs;

public class PlayerStatResponse
{
    public int Place { get; set; }
    public double Score { get; set; }

    // points earned for the current question (0 if wrong or not answered)
    public double RoundPoints { get; set; }
}