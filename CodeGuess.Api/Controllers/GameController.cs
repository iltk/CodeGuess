using CodeGuess.Api.DTOs;
using CodeGuess.Api.Models;
using CodeGuess.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeGuess.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    public GameController(IGameService gameService)
    {
        _gameService = gameService;
        
    }
    

    // POST api/game/3
    [HttpPost("{deckId:int}")] 
    public async Task<ActionResult<GameSessionDto>> Create(int deckId)
    {
        var session = await _gameService.CreateGameAsync(deckId);

        if (session == null)
            return NotFound($"Deck {deckId} not found");

        return Ok(session);
    }
    
    [HttpPost("{gameCode}/join")]
    public async Task<ActionResult> JoinGame(JoinGameDto dto)
    {
        await _gameService.JoinGameAsync(dto);
        return Ok();
    }

   

    
    
    
    
    
    
    
    
    
    
    
}