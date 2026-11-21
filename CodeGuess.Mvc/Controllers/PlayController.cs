using CodeGuess.Mvc.DTOs;
using CodeGuess.Mvc.Services;
using CodeGuess.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CodeGuess.Mvc.Controllers;

public class PlayController : Controller
{
    
    private readonly IGameService _gameService;                                                                                                                                        
                                                                                                                                                                                     
    public PlayController(IGameService gameService)                                                                                                                                    
    {                                                                                                                                                                                  
        _gameService = gameService;                                                                                                                                                    
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
            JoinCode = model.JoinCode,
            Nickname = model.Nickname
        };

        var (playerId, error) = await _gameService.JoinGameAsync(request);

        if (playerId == null)
        {
            TempData["Error"] = error;
            return View("Index", model);
        }

        return RedirectToAction("Game", new { joinCode = model.JoinCode, playerId });
    }

    [HttpGet]
    public IActionResult Game(string joinCode, int playerId)
    {
        var model = new PlayGameViewModel
        {
            JoinCode = joinCode,
            PlayerId = playerId
        };

        return View(model);
    }
}