using CodeGuess.Api.Models;
using CodeGuess.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeGuess.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeckController : ControllerBase
{
    private readonly IDeckService _deckService;
    public DeckController(IDeckService deckService)
    {
        _deckService = deckService;
    }
    
    
    [HttpGet]
    public async Task<ActionResult<List<Deck>>> GetDecks()
    {
        return Ok(await _deckService.GetDecksAsync());
    }
    

   
    
    
    
    
    
}