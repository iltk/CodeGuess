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
    public async Task<IActionResult> Join(JoinGameRequest request)                                                                                                                             
    {                  
        var playerId = await _gameService.JoinGameAsync(request);

        if (playerId == null)
        {
            TempData["Error"] = "Could not join. Check the game code, or try another nickname.";
            return View("Index", request);
        }

        return RedirectToAction("Game", new { joinCode = request.JoinCode, playerId });
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
    
    //players joins lobby - state lobby
    // then host starts game - state - question
    // players see the answer option
    //player clicks on answer, and the whole screen becomes this btn
    //times run out host shows correct and switches to leaderboard
        //player screen also updated green/red depdning on correct
    // host clicks next and repeat 
    
    
}