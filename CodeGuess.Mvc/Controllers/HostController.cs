using CodeGuess.Mvc.Models;
using CodeGuess.Mvc.Services;
using CodeGuess.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CodeGuess.Mvc.Controllers;

public class HostController : Controller
{
    private readonly IDeckService _deckService;
    private readonly IGameService _gameService;

    public HostController(IDeckService deckService, IGameService gameService)
    {
        _deckService = deckService;
        _gameService = gameService;
    }


    public async Task<IActionResult> Index() 
    {
        var decks = await _deckService.GetDecksAsync();

        var model = decks
            .Select(deck => new DeckViewModel { Id = deck.Id, Name = deck.Name })
            .ToList();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int deckId)
    {
        if (deckId <= 0)
        {
            TempData["Error"] = "Please pick a deck.";
            return RedirectToAction("Index");
        }

        var game = await _gameService.CreateGameAsync(deckId);

        if (game == null)
        {
            TempData["Error"] = "Could not create the game. Try again.";
            return RedirectToAction("Index");
        }

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
        if (!await _gameService.StartGameAsync(joinCode))
        {
            TempData["Error"] = "Could not start the game. Try again.";
            return RedirectToAction("Lobby", new { joinCode });
        }

        return RedirectToAction("Question", new { joinCode });
    }

    [HttpGet]
    public async Task<IActionResult> Question(string joinCode)
    {
        var game = await _gameService.GetGameAsync(joinCode);
        if (game == null)
        {
            TempData["Error"] = "Could not load the question.";
            return RedirectToAction("Index");
        }

        var model = new HostQuestionViewModel
        {
            JoinCode = game.JoinCode,
            CurrentQuestionPosition = game.CurrentQuestionPosition,
            TotalQuestions = game.TotalQuestions,
            Code = game.Question?.Code,
            AnswerOptions = game.AnswerOptions?
                .Select(option => new AnswerOptionViewModel { Id = option.Id, Text = option.Text })
                .ToList() ?? new()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> LeaderBoard(string joinCode, GameState state)
    {
        var players = await _gameService.GetLeaderboardAsync(joinCode);
        if (players == null)
        {
            TempData["Error"] = "Could not load the leaderboard.";
            return RedirectToAction("Index");
        }

        var model = new LeaderBoardViewModel
        {
            JoinCode = joinCode,
            IsFinished = state == GameState.Finished,
            Players = players
                .Select(player => new LeaderBoardPlayerViewModel
                {
                    Nickname = player.Nickname,
                    Score = player.Score ?? 0
                })
                .ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Advance(string joinCode)
    {
        var state = await _gameService.AdvanceGameAsync(joinCode);

        if (state == null)
        {
            TempData["Error"] = "Could not go to the next question. Try again.";
            return RedirectToAction("LeaderBoard", new { joinCode });
        }

        if (state == GameState.Finished)
            return RedirectToAction("LeaderBoard", new { joinCode, state });

        return RedirectToAction("Question", new { joinCode });
    }
}
