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
    private readonly IAnswerService _answerService;
    private readonly ILeaderboardService _leaderboardService;

    public GameController(
        IGameService gameService,
        IAnswerService answerService,
        ILeaderboardService leaderboardService)
    {
        _gameService = gameService;
        _answerService = answerService;
        _leaderboardService = leaderboardService;
    }


    // POST api/game   body: { "deckId": 3 }
    [HttpPost]
    public async Task<ActionResult<GameResponse>> CreateGame(CreateGameRequest request)
    {
        var game = await _gameService.CreateGameAsync(request.DeckId);
        
        //if deck doesnt exists, game cant be created
        if (game == null)
            return NotFound();
        
        return Ok(game);
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
            var leaderboard = await _leaderboardService.GetLeaderboardAsync(joinCode);
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
            var correctAnswerId = await _answerService.GetCorrectAnswerIdAsync(joinCode);
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
            var isCorrect = await _answerService.SubmitAnswerAsync(joinCode, request.PlayerId, request.AnswerId);
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
    
    
    [HttpGet("{joinCode}/players/{playerId}/stat")]
    public async Task<ActionResult<PlayerStatResponse>> GetPlayerStatRound(string joinCode, int playerId) 
    {

        try
        {
            var playerStat = await _leaderboardService.GetPlayerStat(joinCode, playerId);
            
            return Ok(playerStat);

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

    // GET api/game/ABCD/players/7/restore
    [HttpGet("{joinCode}/players/{playerId}/restore")]
    public async Task<ActionResult<PlayerRestoreResponse>> GetPlayerRestore(string joinCode, int playerId)
    {
        try
        {
            var restore = await _gameService.GetPlayerRestoreAsync(joinCode, playerId);
            return Ok(restore);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }



}
