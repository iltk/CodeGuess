namespace CodeGuess.Api.Models;


public enum GameState
{
    Lobby = 0, 
    Question = 1,
    CorrectAnswer = 2,
    Leaderboard =  3,
    Finished = 4
}

