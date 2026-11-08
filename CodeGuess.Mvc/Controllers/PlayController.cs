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
    public async Task<IActionResult> Join(JoinGameDto dto)                                                                                                                             
    {                  
        // TODO: return a better msg screen
        if (!await _gameService.JoinGameAsync(dto))                                                                                                                                    
            return View("Index", dto);                                                                                                                                                 
                                                                                                                                                                                     
        return RedirectToAction("Game", new { code = dto.Code });                                                                                                                      
    }     

    
    public IActionResult Game(string code)
    {
        ViewData["Code"] = code;

        return View();
    }
}