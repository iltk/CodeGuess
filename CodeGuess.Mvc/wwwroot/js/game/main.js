// Player game page (Views/Play/Game.cshtml). Entry point: the only file in a <script type="module"> tag.
// Decides which screen to show from the game state, and starts the SignalR connection.
//
//   api.js     talks to the server (fetch)
//   views.js   changes the page (DOM)
//   format.js  small helpers (ordinal, points)
//   state.js   shared constants and variables

import { API_BASE, getGame, getStat, sendAnswer } from "./api.js";
import { GameState, state } from "./state.js";
import {
    showSection, showRoundView, onAnswerPicked, renderAnswerButtons, showQuestionLabel, showSubmittedAnswer,
    showAnswerResult, showTimeUp, showStats, showGain, showFinal, statTargets,
} from "./views.js";

// The join code and player id come from data- attributes on #game
const { joinCode, playerId } = document.getElementById("game").dataset;



// show locked-in partial view right away
function submitAnswer(option, index) {
    showSubmittedAnswer(index);
    state.answerRequest = sendAnswer(joinCode, playerId, option.id);   // promise
}

// show the result section
async function showRoundResult(text, isCorrect) {
    showAnswerResult(text, isCorrect);
    
    const stat = await getStat(joinCode, playerId);
    if (!stat) return;
    showStats(stat, statTargets.result);
    showGain(stat.roundPoints);
}

// show the time's up partial view, then fill the numbers
async function showRoundTimeUp() {
    showTimeUp();

    const stat = await getStat(joinCode, playerId);
    if (stat) showStats(stat, statTargets.timeUp);
}

// show the finsihied partial view, 
async function showFinished() {
    showSection(GameState.Finished);

    const stat = await getStat(joinCode, playerId);
    if (stat) showFinal(stat);
}

// get game state from the server and render ui accordingly
async function showCurrentGame() {
    const callId = ++state.newestCallId;

    // if the player just answered, let it finish saving first
    await state.answerRequest;

    const game = await getGame(joinCode, playerId);
    if (!game) return;

    // a newer call started while this one was waiting, so this data is old
    if (callId !== state.newestCallId) return;

    if (game.state === GameState.Lobby) return showSection(GameState.Lobby);
    if (game.state === GameState.Finished) return showFinished();

    if (game.waitingForTimer) return showRoundView("get-ready-view");

    const pickedIndex = game.answerOptions.findIndex(option => option.id === game.answerId);
    const picked = game.answerOptions[pickedIndex];

    if (picked && game.isTimeUp) return showRoundResult(picked.text, game.isCorrect);
    if (picked) return showSubmittedAnswer(pickedIndex);
    if (game.isTimeUp) return showRoundTimeUp();

    showQuestionLabel(game.questionNumber, game.questionCount);
    renderAnswerButtons(game.answerOptions);
    showRoundView("answering-view");
}



const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${API_BASE}/hubs/game`)
    .build();

// showCurrentGame gets the details from the server and updates the ui
connection.on("StateChanged", showCurrentGame);
connection.on("QuestionStarted", showCurrentGame);
connection.on("TimerStopped", showCurrentGame);

// attaches one click listener to each of the four A–D buttons
onAnswerPicked(submitAnswer);

// Connect, join this room's group, then rebuild the page from the server
connection.start()
    .then(async () => {
        await connection.invoke("JoinGame", joinCode);

        //handles user reload, restores previous page
        await showCurrentGame();
    })
    .catch(err => console.error(err));
