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
    public const int QuestionSeconds = 10;

    private readonly AppDbContext _db;
    private readonly IHubContext<GameHub> _hub;


    public GameService(AppDbContext db,  IHubContext<GameHub> hub)
    {
        _db = db;
        _hub = hub;

    }

    public async Task<GameResponse?> CreateGameAsync(int deckId)
    {
        //return null if deck doesnt exists
        if (!await _db.Decks.AnyAsync(d => d.Id == deckId))
            return null;
        
        int totalQuestions = await CountQuestionsAsync(deckId);

        var session = new GameSession
        {
            DeckId = deckId,
            JoinCode = await GenerateUniqueJoinCodeAsync()
        };


        _db.GameSessions.Add(session);
        await _db.SaveChangesAsync();

        return new GameResponse
        {
            Id = session.Id,
            QuestionStartedAt = session.QuestionStartedAt,
            DeckId = session.DeckId,
            JoinCode = session.JoinCode,
            State = session.State,
            CurrentQuestionPosition = session.CurrentQuestionPosition,
            TotalQuestions = totalQuestions
        };


    }

    public async Task<int> JoinGameAsync(string joinCode, JoinGameRequest request)
    {
        var session = await _db.GetSessionAsync(joinCode);

        if (session == null)
        {
            throw new InvalidOperationException("This game doesn't exist.");
        }

        if (session.State != GameState.Lobby)
            throw new InvalidOperationException("This game has already started.");

        var nicknameTaken = await _db.Players
            .AnyAsync(p => p.GameSessionId == session.Id && p.Nickname == request.Nickname);

        if (nicknameTaken)
            throw new InvalidOperationException("This nickname is already taken.");

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

        return newPlayer.Id;

    }

    public async Task StartGameAsync(string joinCode)
    {
        var session = await _db.GetSessionAsync(joinCode);

        if (session.State != GameState.Lobby)
            throw new InvalidOperationException("This game has already started.");

        session.State = GameState.Question;
        await _db.SaveChangesAsync();

        await _hub.Clients.Group(joinCode).SendAsync("StateChanged", session.State); // tell everyone in that room

        // the answer options are sent by GameTimerService ("QuestionStarted") when the host's timer starts
    }

    public async Task<GameResponse> GetGameAsync(string joinCode)
    {
        var session = await _db.GetSessionAsync(joinCode);

        var question = await _db.Questions
            .FirstOrDefaultAsync(q => q.DeckId == session.DeckId
                                   && q.Position == session.CurrentQuestionPosition);

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
            QuestionStartedAt = session.QuestionStartedAt,
            CurrentQuestionPosition = session.CurrentQuestionPosition,
            TotalQuestions = await CountQuestionsAsync(session.DeckId),
            Question = question == null ? null : new QuestionResponse
            {
                Id = question.Id,
                Position = question.Position,
                Code = question.Code
            },
            AnswerOptions = answers
        };

    }

    public async Task<GameState> AdvanceGameAsync(string joinCode)
    {
        var session = await _db.GetSessionAsync(joinCode);

        if (session.State == GameState.Finished)
            throw new InvalidOperationException("This game is already finished.");

        var totalQuestions = await CountQuestionsAsync(session.DeckId);

        if (totalQuestions <= session.CurrentQuestionPosition + 1)
        {
            // No questions left, the game is over
            session.State = GameState.Finished;
        }
        else
        {
            session.CurrentQuestionPosition++;
            session.QuestionStartedAt = null; // the next question starts when the host's timer does
        }

        await _db.SaveChangesAsync();

        await _hub.Clients.Group(joinCode).SendAsync("StateChanged", session.State);   // tell everyone in that room

        return session.State;

    }

    //used in start timer service when timer starts it marks the time stamp when it was created
    public async Task MarkQuestionStartedAsync(string joinCode)
    {
        var session = await _db.GetSessionAsync(joinCode);
        session.QuestionStartedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    // Everything the player page needs to rebuild itself after a reload
    public async Task<PlayerRestoreResponse> GetPlayerRestoreAsync(string joinCode, int playerId)
    {
        var session = await _db.GetSessionAsync(joinCode);

        var player = await _db.Players
            .FirstOrDefaultAsync(p => p.Id == playerId && p.GameSessionId == session.Id);

        if (player == null)
            throw new KeyNotFoundException($"Player {playerId} not found in this game.");

        var response = new PlayerRestoreResponse { State = session.State };

        // only a running question has options to show
        if (session.State != GameState.Question)
            return response;

        if (session.QuestionStartedAt == null)
        {
            response.WaitingForTimer = true;
            return response;
        }

        var isTimeUp = (DateTime.UtcNow - session.QuestionStartedAt.Value).TotalSeconds >= QuestionSeconds;

        if (isTimeUp)
            response.State = GameState.CorrectAnswer;

        var question = await _db.GetCurrentQuestionAsync(session);

        var options = await _db.AnswerOptions
            .Where(a => a.QuestionId == question.Id)
            .ToListAsync();

        response.AnswerOptions = options
            .Select(a => new AnswerOptionResponse { Id = a.Id, Text = a.Text })
            .ToList();

        // LastAnswerId only counts if it belongs to the current question
        if (player.LastAnsweredPosition == session.CurrentQuestionPosition)
        {
            var picked = options.FirstOrDefault(a => a.Id == player.LastAnswerId);
            response.AnswerId = picked?.Id;
            response.IsCorrect = picked?.IsCorrect;
        }

        return response;
    }


    //helpers

    private Task<int> CountQuestionsAsync(int deckId)
    {
        return _db.Questions.CountAsync(q => q.DeckId == deckId);
    }

    // Keeps generating codes until one isn't used by another game
    private async Task<string> GenerateUniqueJoinCodeAsync()
    {
        string joinCode;

        do
        {
            joinCode = GenerateJoinCode();
        }
        while (await _db.GameSessions.AnyAsync(s => s.JoinCode == joinCode));

        return joinCode;
    }

    //creates the short codes players type in to join a game
    private static string GenerateJoinCode()
    {
        const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const int joinCodeLength = 4;

        return new string(
            Enumerable.Range(0, joinCodeLength)
                .Select(_ => allowedChars[RandomNumberGenerator.GetInt32(allowedChars.Length)])
                .ToArray()
        );
    }
}
