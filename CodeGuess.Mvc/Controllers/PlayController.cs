using CodeGuess.Mvc.DTOs;
using CodeGuess.Mvc.Services;
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
        // TODO: return a better msg screen
        if (!await _gameService.JoinGameAsync(request))                                                                                                                                    
            return View("Index", request);                                                                                                                                                 
                                                                                                                                                                                     
        return RedirectToAction("Game", new { joinCode = request.JoinCode });                                                                                                                      
    }     

    
    public IActionResult Game(string joinCode)
    {
        ViewData["JoinCode"] = joinCode;

        return View();
    }
}