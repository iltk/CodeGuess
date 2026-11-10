const joinCode = document.getElementById("game").dataset.joinCode;

// Must match the GameState enum in the API
const GameState = { Lobby: 0, Question: 1, Results: 2, Leaderboard: 3, Finished: 4 };

const sections = {
    [GameState.Lobby]: "lobby-section",
    [GameState.Question]: "question-section",
    [GameState.Results]: "result-section"
};

const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5129/hubs/game")
    .build();

// Server sends this when the host moves the game to a new state
connection.on("StateChanged", state => showSection(state));

// Connect, then join this room's group
connection.start()
    .then(() => connection.invoke("JoinGame", joinCode))
    .catch(err => console.error(err));

function showSection(state) {
    document.querySelectorAll(".tab-content").forEach(section => {
        section.classList.remove("d-block");
        section.classList.add("d-none");
    });

    const target = document.getElementById(sections[state]);
    if (target) {
        target.classList.remove("d-none");
        target.classList.add("d-block");
    }
}
