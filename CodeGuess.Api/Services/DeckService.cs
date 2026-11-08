using CodeGuess.Api.Data;
using CodeGuess.Api.DTOs;
using CodeGuess.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeGuess.Api.Services;

public class DeckService : IDeckService
{
    private readonly AppDbContext _db;

    public DeckService(AppDbContext db)
    {
        _db = db;
    }
    
    
    public async Task<List<Deck>> GetDecksAsync()
    {
        return await _db.Decks.ToListAsync();

    }
    
   
    
    
}