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

    public async Task<GameResponse> CreateGameAsync(int deckId)
    {
        if (!await _db.Decks.AnyAsync(d => d.Id == deckId))
            throw new KeyNotFoundException($"Deck {deckId} not found.");

        var session = new GameSession
        {
            DeckId = deckId,
            JoinCode = await GenerateUniqueJoinCodeAsync()
        };


        _db.GameSessions.Add(session);
        await _db.SaveChangesAsync();

        var response = new GameResponse
        {
            Id = session.Id,
            QuestionStartedAt = session.QuestionStartedAt,
            DeckId = session.DeckId,
            JoinCode = session.JoinCode,
            State = session.State,
            CurrentQuestionPosition = session.CurrentQuestionPosition,
            TotalQuestions = await CountQuestionsAsync(session.DeckId)
        };

        return response;

    }
    
    public async Task<int> JoinGameAsync(string joinCode, JoinGameRequest request)
    {
        var session = await GetSessionAsync(joinCode);

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
        var session = await GetSessionAsync(joinCode);

        if (session.State != GameState.Lobby)
            throw new InvalidOperationException("This game has already started.");

        session.State = GameState.Question;
        await _db.SaveChangesAsync();

        await _hub.Clients.Group(joinCode).SendAsync("StateChanged", session.State); // tell everyone in that room

        // the answer options are sent by GameTimerService ("QuestionStarted") when the host's timer starts
    }

    public async Task<GameResponse> GetGameAsync(string joinCode)
    {
        var session = await GetSessionAsync(joinCode);

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

    public async Task<List<PlayerResponse>> GetLeaderboardAsync(string joinCode)
    {
        var session = await GetSessionAsync(joinCode);

        //get players and sort them by their score
        var players = await _db.Players
            .Where(p => p.GameSessionId == session.Id)
            .OrderByDescending(p => p.Score)
            .ToListAsync();

        var leaderboard = new List<PlayerResponse>();

        foreach (Player player in players)
        {
            leaderboard.Add(new PlayerResponse
            {
                Id = player.Id,
                Nickname = player.Nickname,
                GameSessionId = session.Id,
                Score = player.Score,

            });

        }

        return leaderboard;
    }
    
    public async Task<int> GetCorrectAnswerIdAsync(string joinCode)
    {
        var session = await GetSessionAsync(joinCode);
        var question = await GetCurrentQuestionAsync(session);

        var correctAnswer = await _db.AnswerOptions
            .FirstOrDefaultAsync(a => a.QuestionId == question.Id && a.IsCorrect);

        if (correctAnswer == null)
            throw new KeyNotFoundException("This question has no correct answer.");

        return correctAnswer.Id;
    }

    public async Task<GameState> AdvanceGameAsync(string joinCode)
    {
        var session = await GetSessionAsync(joinCode);

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
        }

        await _db.SaveChangesAsync();

        await _hub.Clients.Group(joinCode).SendAsync("StateChanged", session.State);   // tell everyone in that room

        return session.State;

    }
    
    // Checks the player's answer for the current question and adds time-based points if it's correct.
    // Each player can answer each question only once.
    public async Task<bool> SubmitAnswerAsync(string joinCode, int playerId, int answerId)
    {
        DateTime receivedAt = DateTime.UtcNow;

        var session = await GetSessionAsync(joinCode);

        var player = await _db.Players
            .FirstOrDefaultAsync(p => p.Id == playerId && p.GameSessionId == session.Id);

        if (player == null)
            throw new KeyNotFoundException($"Player {playerId} not found in this game.");

        var question = await GetCurrentQuestionAsync(session);

        var answer = await _db.AnswerOptions
            .FirstOrDefaultAsync(a => a.Id == answerId && a.QuestionId == question.Id);

        if (answer == null)
            throw new InvalidOperationException("This answer is not an option for the current question.");

        if (player.LastAnsweredPosition == session.CurrentQuestionPosition)
            throw new InvalidOperationException("You already answered this question.");

        // remember this question is answered, whether the answer is right or wrong
        player.LastAnsweredPosition = session.CurrentQuestionPosition;

        if (!answer.IsCorrect)
        {
            await _db.SaveChangesAsync();
            return false;
        }

        // formula taken from Kahoot 
        //Points=( 1−  Response Time /Total Time for Question  ×0.5)× Max Points
        //response  time: time that took player to answer  
        double responseTime = (double)((receivedAt - session.QuestionStartedAt)?.TotalSeconds ?? 0);

        if (responseTime == 0)
        {
            throw new InvalidOperationException("Time must be greater than zero.");
        }
        
        //max points that player can get per question
        double maxPoints = 1000;
        double responseRatio = responseTime / 10;
        
        //award player full points 
        if (responseTime <= 0.5) {
            responseRatio = 0;
        }
        
        double score = Math.Round((1 - (responseRatio * 0.5)) * maxPoints);
        
        player.Score += score;
        await _db.SaveChangesAsync();
        return true;
    }


    //helpers

    //used in start timer service when timer starts it marks the time stamp when it was created
    public async Task MarkQuestionStartedAsync(string joinCode)    
    
    {                                                                                                                                                                                  
        var session = await GetSessionAsync(joinCode);                                                                                                                                 
        session.QuestionStartedAt = DateTime.UtcNow;                                                                                                                                   
        await _db.SaveChangesAsync();                                                                                                                                                  
    }
    
    
    public async Task<int> GetPlayerPlace(string joinCode, int playerId)
    {
        
        var session = await GetSessionAsync(joinCode);
        
        var players = await _db.Players
            .Where(p => p.GameSessionId == session.Id)
            .OrderByDescending(p => p.Score)
            .ToListAsync();

        int playerPosition = 1;
        
        foreach (Player player in players)
        {
            if (player.Id == playerId)
            {
                return playerPosition;
            }
            playerPosition++;

        }

        return 0;
        
    }

   

    private async Task<GameSession> GetSessionAsync(string joinCode)
    {
        var session = await _db.GameSessions
            .FirstOrDefaultAsync(s => s.JoinCode == joinCode);

        if (session == null)
            throw new KeyNotFoundException($"Game {joinCode} not found.");

        return session;
    }

    // The question the game is on right now
    private async Task<Question> GetCurrentQuestionAsync(GameSession session)
    {
        var question = await _db.Questions
            .FirstOrDefaultAsync(q => q.DeckId == session.DeckId
                                   && q.Position == session.CurrentQuestionPosition);

        if (question == null)
            throw new KeyNotFoundException("The current question was not found.");

        return question;
    }

    public async Task<List<AnswerOptionResponse>> GetAnswerOptionsAsync(string joinCode)
    {
        var session = await GetSessionAsync(joinCode);

        var question = await _db.Questions
            .FirstOrDefaultAsync(q => q.DeckId == session.DeckId
                                      && q.Position == session.CurrentQuestionPosition);

        var answers = question == null
            ? new List<AnswerOptionResponse>()
            : await _db.AnswerOptions
                .Where(a => a.QuestionId == question.Id)
                .Select(a => new AnswerOptionResponse { Id = a.Id, Text = a.Text })
                .ToListAsync();
        return answers;

    }

    

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
