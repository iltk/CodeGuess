namespace CodeGuess.Mvc.DTOs;

//  player's answer   
public class SubmitAnswerRequest
{

    public int PlayerId { get; set; }

    public int AnswerId { get; set; }
    
}
