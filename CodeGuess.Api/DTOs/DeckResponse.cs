namespace CodeGuess.Api.DTOs;

// What the API sends back about a deck (returned by GET api/deck)
public class DeckResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
