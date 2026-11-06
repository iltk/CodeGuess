using CodeGuess.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeGuess.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Deck> Decks { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<AnswerOption> AnswerOptions { get; set; }
}