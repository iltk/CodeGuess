using CodeGuess.Api.Data;
using CodeGuess.Api.DTOs;
using CodeGuess.Api.Models;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;


namespace CodeGuess.Api.Services;

public class GameService: IGameService
{
    private readonly AppDbContext _db;

    public GameService(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<GameSessionDto?> CreateGameAsync(int deckId)
    {
        
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