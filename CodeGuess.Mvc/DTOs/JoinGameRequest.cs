namespace CodeGuess.Mvc.DTOs;

public class JoinGameRequest
{
    public string JoinCode { get; set; } = string.Empty;
    
    public string Nickname { get; set; } = string.Empty;
}