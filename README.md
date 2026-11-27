# CodeGuess

A Kahoot-style quiz for programmers. The host picks a deck and opens a game. Players join from their phones with a 4-character code, and each round they see a code snippet and pick the right answer before the 10-second timer runs out. Faster correct answers score more.


## How a game works

1. **Host** picks a deck on the host page and creates a game. The API generates a unique join code.
2. **Players** enter the code and a nickname. Nicknames must be unique within a game.
3. The host's lobby updates live as players join (SignalR `PlayerJoined`).
4. The host starts the game. Each round:
   - the server starts a 10-second timer and pushes it to everyone in the game (`QuestionStarted`, `TimerStarted`, `TimerStopped`)
   - players pick an answer and get a correct, incorrect or "time's up" screen
   - the correct answer is revealed, then a leaderboard
5. After the last question, the host sees the final results and each player sees their score and place.

Reloading the page mid-game puts a player back where they were, because the player screen renders from server state.

### Scoring

Based on Kahoot's formula, with a maximum of 1000 points per question:

```
points = (1 − (responseTime / 10) × 0.5) × 1000
```

Answering within 0.5 s gives full points, and a correct answer always gives at least 500. Wrong answers score 0.

## Architecture

One solution, two ASP.NET Core 8 projects:

```
CodeGuess.sln
├── CodeGuess.Api   Web API + SignalR hub + EF Core (SQLite)
│   ├── Controllers/   DeckController, GameController
│   ├── Hubs/          GameHub (/hubs/game)
│   ├── Services/      GameService, AnswerService, LeaderboardService,
│   │                  GameTimerService, ScoreCalculator, DeckService
│   ├── Models/        Deck → Question → AnswerOption, GameSession, Player
│   ├── Data/          AppDbContext, DbSeeder
│   └── Migrations/
└── CodeGuess.Mvc   MVC front end (Razor views + Bootstrap + ES modules)
    ├── Controllers/   HostController, PlayController
    ├── Services/      typed HttpClients that call the API
    ├── ViewModels/
    └── Views/         Host/* (lobby, round, leaderboard, final)
                       Play/* (join, game + partials per screen)
```

- The **MVC app never touches the database**. It calls the API over HTTP (`ApiBaseUrl` in `appsettings.json`).
- The **browser connects straight to the API's SignalR hub** for real-time events. The API allows the MVC origin through CORS.
- **Game state** (`Lobby`, `InProgress`, `Finished`) lives on the `GameSession` in the database, so any page can be rebuilt from it.

### API endpoints

| Method | Route | Purpose |
|---|---|---|
| GET | `/api/deck` | list decks |
| POST | `/api/game` | create a game from a deck |
| POST | `/api/game/{joinCode}/join` | join with a nickname, returns the player id |
| POST | `/api/game/{joinCode}/start` | start the game |
| GET | `/api/game/{joinCode}` | current game state and question |
| POST | `/api/game/{joinCode}/answer` | submit an answer |
| GET | `/api/game/{joinCode}/correct-answer` | reveal the correct answer |
| GET | `/api/game/{joinCode}/leaderboard` | scores so far |
| POST | `/api/game/{joinCode}/advance` | next question or finish |
| GET | `/api/game/{joinCode}/players/{playerId}/stat` | a player's score, place and last-round points |
| GET | `/api/game/{joinCode}/players/{playerId}/game` | a player's view of the game |

Swagger UI is at `/swagger` when the API runs in Development.


## Tech

- ASP.NET Core 8 (Web API + MVC)
- Entity Framework Core 8 with SQLite
- SignalR for live lobby updates, round timers and state changes
- Bootstrap with custom design tokens, vanilla JS (ES modules)
- Swagger / Swashbuckle

## Work tracking

Features and bugs are tracked as GitHub issues on the project board, and each one is closed by a PR.
