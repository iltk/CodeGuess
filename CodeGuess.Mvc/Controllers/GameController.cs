using CodeGuess.Mvc.Models;
using CodeGuess.Mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeGuess.Mvc.Controllers;

public class GameController : Controller
{
    private readonly IDeckService _deckService;
    private readonly IGameService _gameService;

    public GameController(IDeckService deckService, IGameService gameService)
    {
        _deckService = deckService;
        _gameService = gameService;
    }


    public async Task<IActionResult> Index() // GET /game
    {
        var decks = await _deckService.GetDecksAsync();

        return View(decks);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(int deckId)
    {
        if (deckId <= 0)
            return RedirectToAction("Index");

        var game = await _gameService.CreateGameAsync(deckId);

        // if API failed
        if (game == null)
            return RedirectToAction("Index");

        return RedirectToAction("Lobby", new { code = game.JoinCode, sessionId = game.Id});
    }
    
    [HttpGet]
    public IActionResult Lobby(string code, int sessionId)
    {
        var model = new LobbyViewModel { Code = code };
        return View(model);
    }

    
    
    
    
}