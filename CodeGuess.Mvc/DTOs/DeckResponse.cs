namespace CodeGuess.Mvc.DTOs;

// used to read the API's JSON response from GET api/deck
public class DeckResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
