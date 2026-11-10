using CodeGuess.Mvc.Models;
using CodeGuess.Mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeGuess.Mvc.Controllers;

// Screens for the person hosting a game
public class HostController : Controller
{
    private readonly IDeckService _deckService;
    private readonly IGameService _gameService;

    public HostController(IDeckService deckService, IGameService gameService)
    {
        _deckService = deckService;
        _gameService = gameService;
    }


    public async Task<IActionResult> Index() // GET /host
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

        return RedirectToAction("Lobby", new { joinCode = game.JoinCode });
    }

    [HttpGet]
    public IActionResult Lobby(string joinCode)
    {
        var model = new LobbyViewModel { JoinCode = joinCode };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Start(string joinCode)
    {
        await _gameService.StartGameAsync(joinCode);

        return RedirectToAction("Question", new { joinCode });
    }

    [HttpGet]
    public async Task<IActionResult> Question(string joinCode)
    {
        var game = await _gameService.GetGameAsync(joinCode);
        if (game == null)
            return RedirectToAction("Index");

        return View(game);
    }

    [HttpGet]
    public async Task<IActionResult> LeaderBoard(string joinCode)                                                                                                                      
    {                                                                                                                                                                                  
        var result = await _gameService.GetLeaderBoardAsync(joinCode);                                                                                                                 
        if (result == null)                                                                                                                                                            
            return RedirectToAction("Index");                                                                                                                                          
                                                                                                                                                                                     
        ViewData["JoinCode"] = joinCode;                                                                                                                                               
        return View(result);                                                                                                                                                           
    }  
}
