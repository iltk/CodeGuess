using CodeGuess.Api.DTOs;
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
        var game = await _gameService.CreateGameAsync(request.DeckId);

        if (game == null)
            return NotFound($"Deck {request.DeckId} not found");

        return Ok(game);
    }

    // POST api/game/ABCD/join   body: { "nickname": "Sam" }
    [HttpPost("{joinCode}/join")]
    public async Task<ActionResult> JoinGame(string joinCode, JoinGameRequest request)
    {
        await _gameService.JoinGameAsync(joinCode, request);
        return Ok();
    }

    // POST api/game/ABCD/start
    [HttpPost("{joinCode}/start")]
    public async Task<ActionResult> StartGame(string joinCode)
    {
        await _gameService.StartGameAsync(joinCode);
        return Ok();
    }

    // GET api/game/ABCD
    [HttpGet("{joinCode}")]
    public async Task<ActionResult<GameResponse>> GetGame(string joinCode)
    {
        var game = await _gameService.GetGameAsync(joinCode);
        return Ok(game);
    }

    // GET api/game/ABCD/leaderboard
    [HttpGet("{joinCode}/leaderboard")]
    public async Task<ActionResult<List<PlayerResponse>>> GetLeaderboard(string joinCode)
    {
        var leaderboard = await _gameService.GetLeaderboardAsync(joinCode);
        return Ok(leaderboard);
    }

    // GET api/game/ABCD/correct-answer?questionId=5
    [HttpGet("{joinCode}/correct-answer")]
    public async Task<ActionResult<int>> GetCorrectAnswerId(string joinCode, int questionId)
    {
        var correctAnswerId = await _gameService.GetCorrectAnswerIdAsync(joinCode, questionId);
        return Ok(correctAnswerId);
    }
}
