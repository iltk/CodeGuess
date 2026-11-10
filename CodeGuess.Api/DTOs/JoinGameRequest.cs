namespace CodeGuess.Api.DTOs;

// Body of POST api/game/{joinCode}/join (the join code comes from the URL)
public class JoinGameRequest
{
    public string Nickname { get; set; } = string.Empty;
}
