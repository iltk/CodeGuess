using CodeGuess.Mvc.DTOs;
using CodeGuess.Mvc.Services;
using CodeGuess.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CodeGuess.Mvc.Controllers;

public class PlayController : Controller
{
    
    private readonly IGameService _gameService;
    private readonly IDeckService _deckService;                                                                                                                                        
                                                                                                                                                                                     
    public PlayController(IGameService gameService, IDeckService deckService)                                                                                                                                    
    {                                                                                                                                                                                  
        _gameService = gameService;
        _deckService = deckService;                                                                                                                                                    
    }                                                                                                                                                                                  

    
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Join(JoinGameViewModel model)
    {
        if (!ModelState.IsValid)  return View("Index", model);

        var request = new JoinGameRequest
        {
            JoinCode = model.JoinCode.Trim().ToUpperInvariant(),              
            Nickname = model.Nickname
        };

        var (playerId, error) = await _gameService.JoinGameAsync(request);

        if (playerId == null)
        {
            TempData["Error"] = error;
            return View("Index", model);
        }

        return RedirectToAction("Game", new { joinCode = request.JoinCode, playerId });
    }

    [HttpGet]
    public async Task<IActionResult> Game(string joinCode, int playerId)
    {
        var game = await _gameService.GetGameAsync(joinCode);
        var players = await _gameService.GetLeaderboardAsync(joinCode);
        var player = players?.FirstOrDefault(p => p.Id == playerId);

        if (game == null || player == null)
        {
            TempData["Error"] = "We couldn't find that game or player.";
            return RedirectToAction("Index");
        }

        var decks = await _deckService.GetDecksAsync();
        var deck = decks.FirstOrDefault(d => d.Id == game.DeckId);

        var model = new PlayGameViewModel
        {
            JoinCode = game.JoinCode,
            PlayerId = playerId,
            Nickname = player.Nickname,
            DeckName = deck?.Name ?? "Unknown deck",
            QuestionCount = game.TotalQuestions
        };

        return View(model);
    }
}