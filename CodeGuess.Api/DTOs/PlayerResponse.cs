namespace CodeGuess.Api.DTOs;

public class PlayerResponse
{
    public int Id { get; set; }

    public string Nickname { get; set; } = string.Empty;

    public int GameSessionId { get; set; }
    
    public int Score { get; set; } = 0;

    
}