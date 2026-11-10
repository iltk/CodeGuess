using CodeGuess.Api.Data;
using CodeGuess.Api.DTOs;
using CodeGuess.Api.Models;
using System.Security.Cryptography;
using CodeGuess.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace CodeGuess.Api.Services;

public class GameService: IGameService
{
    private readonly AppDbContext _db;
    private readonly IHubContext<GameHub> _hub;


    public GameService(AppDbContext db,  IHubContext<GameHub> hub)
    {
        _db = db;
        _hub = hub;
        
    }
    
    public async Task<GameResponse?> CreateGameAsync(int deckId)
    {
        //TODO: check for the uniqe code generetion
        var session = new GameSession
        {
            DeckId = deckId,
            JoinCode = GenerateJoinCode(),
            CreatedAt = DateTime.UtcNow
        };

        
        _db.GameSessions.Add(session);
        await _db.SaveChangesAsync();

        var response = new GameResponse
        {
            Id = session.Id,
            CreatedAt = session.CreatedAt,
            DeckId = session.DeckId,
            JoinCode = session.JoinCode,
            State = session.State,

        };

        return response;

    }


    public async Task JoinGameAsync(string joinCode, JoinGameRequest request)
    {
        GameSession? session = await _db.GameSessions
            .FirstOrDefaultAsync(s => s.JoinCode == joinCode);

        if (session == null)
        {
            throw new KeyNotFoundException("Game session not found.");
        }

        var newPlayer = new Player
        {
            Nickname = request.Nickname,
            GameSessionId = session.Id,
            Score = 0,
            JoinedAt = DateTime.UtcNow
        };

        _db.Players.Add(newPlayer);
        await _db.SaveChangesAsync();
        
        //Broadcasts an update to everyone subscribed
        await _hub.Clients.Group(joinCode).SendAsync("PlayerJoined", request.Nickname);   // tell everyone in that room

    }

    public async Task StartGameAsync(string joinCode)
    {
        var session = await _db.GameSessions
            .FirstOrDefaultAsync(s => s.JoinCode == joinCode);
        
        if (session == null)
        {
            throw new KeyNotFoundException("Game session not found.");
        }
        session.State = GameState.Question;
        await _db.SaveChangesAsync();
        
        await _hub.Clients.Group(joinCode).SendAsync("StateChanged", session.State);   // tell everyone in that room

        
    }

    public async Task<GameResponse> GetGameAsync(string joinCode)
    {
        var session = await _db.GameSessions
            .FirstOrDefaultAsync(s => s.JoinCode == joinCode);

        if (session == null)
        {
            throw new KeyNotFoundException("Game session not found.");
        }

        var question = await _db.Questions
            .FirstOrDefaultAsync(q => q.DeckId == session.DeckId
                                   && q.Position == session.CurrentQuestionPosition);

        // Select straight into DTOs so IsCorrect never leaves the server
        var answers = question == null
            ? new List<AnswerOptionResponse>()
            : await _db.AnswerOptions
                .Where(a => a.QuestionId == question.Id)
                .Select(a => new AnswerOptionResponse { Id = a.Id, Text = a.Text })
                .ToListAsync();

        return new GameResponse
        {
            Id = session.Id,
            JoinCode = session.JoinCode,
            DeckId = session.DeckId,
            State = session.State,
            CreatedAt = session.CreatedAt,
            Question = question == null ? null : new QuestionResponse
            {
                Id = question.Id,
                Position = question.Position,
                Code = question.Code
            },
            AnswerOptions = answers
        };
    }

    public async Task<List<PlayerResponse>> GetLeaderBoardAsync(string joinCode)
    {
        var session = await _db.GameSessions
            .FirstOrDefaultAsync(s => s.JoinCode == joinCode);
        
        if (session == null)
        {
            throw new KeyNotFoundException("Game session not found."); 
        }

        //get players and sort them by their score 
        var players = await _db.Players
            .Where(p => p.GameSessionId == session.Id)
            .OrderByDescending(p => p.Score)
            .ToListAsync();

        List<PlayerResponse> playerList = new List<PlayerResponse>();

        foreach (var player in players)
        {
            playerList.Add(new PlayerResponse
            {
                Id = player.Id,
                Nickname = player.Nickname,
                GameSessionId = session.Id,
                Score = player.Score,

            });

        }
        
        return playerList;
    }
    



    //creates the short codes players type in to join a game                                                                                                                          
    private static string GenerateJoinCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        return new string(
            Enumerable.Range(0, 4)
                .Select(_ => chars[RandomNumberGenerator.GetInt32(chars.Length)])
                .ToArray()
        );
    }
    
    
}