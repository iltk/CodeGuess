using CodeGuess.Api.Data;
using CodeGuess.Api.DTOs;
using CodeGuess.Api.Models;
using System.Security.Cryptography;
using CodeGuess.Api.Hub;
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
    
    public async Task<GameSessionDto?> CreateGameAsync(int deckId)
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

        GameSessionDto dto = new GameSessionDto
        {
            Id = session.Id,
            CreatedAt = session.CreatedAt,
            DeckId = session.DeckId,
            JoinCode = session.JoinCode,
            State = session.State,
            CurrentQuestionIndex = session.CurrentQuestionIndex

        };

        return dto;

    }


    public async Task JoinGameAsync(JoinGameDto dto)
    {
        GameSession? session = await _db.GameSessions
            .FirstOrDefaultAsync(s => s.JoinCode == dto.Code);

        if (session == null)
        {
            throw new KeyNotFoundException("Game session not found.");
        }

        var newPlayer = new Player
        {
            Nickname = dto.Nickname,
            GameSessionId = session.Id,
            Score = 0,
            JoinedAt = DateTime.UtcNow
        };

        _db.Players.Add(newPlayer);
        await _db.SaveChangesAsync();
        
        //Broadcasts an update to everyone subscribed
        await _hub.Clients.Group(dto.Code).SendAsync("MembersChanged", dto.Nickname);   // tell everyone in that room

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