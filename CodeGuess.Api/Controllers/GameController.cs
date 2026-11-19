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


    // POST api/game   body: { "deckId": 3 }
    [HttpPost]
    public async Task<ActionResult<GameResponse>> CreateGame(CreateGameRequest request)
    {
        try
        {
            var game = await _gameService.CreateGameAsync(request.DeckId);
            return Ok(game);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // POST api/game/ABCD/join   body: { "nickname": "JJJ" }
    [HttpPost("{joinCode}/join")]
    public async Task<ActionResult<int>> JoinGame(string joinCode, JoinGameRequest request)
    {
        try
        {
            var playerId = await _gameService.JoinGameAsync(joinCode, request);
            return Ok(playerId);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST api/game/ABCD/start
    [HttpPost("{joinCode}/start")]
    public async Task<ActionResult> StartGame(string joinCode)
    {
        try
        {
            await _gameService.StartGameAsync(joinCode);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // GET api/game/ABCD
    [HttpGet("{joinCode}")]
    public async Task<ActionResult<GameResponse>> GetGame(string joinCode)
    {
        try
        {
            var game = await _gameService.GetGameAsync(joinCode);
            return Ok(game);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // GET api/game/ABCD/leaderboard
    [HttpGet("{joinCode}/leaderboard")]
    public async Task<ActionResult<List<PlayerResponse>>> GetLeaderboard(string joinCode)
    {
        try
        {
            var leaderboard = await _gameService.GetLeaderboardAsync(joinCode);
            return Ok(leaderboard);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // GET api/game/ABCD/correct-answer
    [HttpGet("{joinCode}/correct-answer")]
    public async Task<ActionResult<int>> GetCorrectAnswerId(string joinCode)
    {
        try
        {
            var correctAnswerId = await _gameService.GetCorrectAnswerIdAsync(joinCode);
            return Ok(correctAnswerId);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // POST api/game/ABCD/advance  
    [HttpPost("{joinCode}/advance")]
    public async Task<ActionResult<GameState>> AdvanceGame(string joinCode)
    {
        try
        {
            var state = await _gameService.AdvanceGameAsync(joinCode);
            return Ok(state);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // POST api/game/ABCD/answer   body: { "playerId": 7, "answerId": 12 }
    [HttpPost("{joinCode}/answer")]
    public async Task<ActionResult<bool>> SubmitAnswer(string joinCode, SubmitAnswerRequest request)
    {
        try
        {
            var isCorrect = await _gameService.SubmitAnswerAsync(joinCode, request.PlayerId, request.AnswerId);
            return Ok(isCorrect);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    
    [HttpGet("{joinCode}/players/{playerId}/place")]
    public async Task<ActionResult<int>> GetPlayerPlace(string joinCode, int playerId) 
    {

        try
        {
            var playerPlace = await _gameService.GetPlayerPlace(joinCode, playerId);
            return Ok(playerPlace);

        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        
        
        
    }



}
