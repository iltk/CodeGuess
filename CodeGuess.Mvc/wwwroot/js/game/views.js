import { GameState } from "./state.js";
import { ordinal, points, questionLabel } from "./format.js";

const sectionIds = {
    [GameState.Lobby]: "lobby-section",
    [GameState.InProgress]: "in-progress-section",
    [GameState.Finished]: "finished-section",
};

// the four A–D buttons, written in _Answering.cshtml
const answerButtons = [...document.querySelectorAll("#answer-buttons .cg-option")];
let currentOptions = [];   // this question's answer options, in A–D order
const questionLabelElement = document.getElementById("question-label");
const submittedLetterElement = document.getElementById("submitted-letter");
const submittedAnswerElement = document.getElementById("submitted-answer");

const resultView = document.getElementById("result-view");
const playerAnswerElement = document.getElementById("player-answer");
const answerResultElement = document.getElementById("answer-result");
const resultGainElement = document.getElementById("result-gain");

// score + place boxes on each screen that shows them
export const statTargets = {
    result:   { score: document.getElementById("score"),        place: document.getElementById("place") },
    timeUp:   { score: document.getElementById("timeup-score"), place: document.getElementById("timeup-place") },
    finished: { score: document.getElementById("final-score"),  place: document.getElementById("final-place") },
};

const finalPlaceBox = document.getElementById("final-place-box");
const finalPodium = document.getElementById("final-podium");

const mainElement = document.querySelector("main");

function show(element) {
    element.hidden = false;
}

function hide(element) {
    element.hidden = true;
}

// hide every section, then show the one for this game state
export function showSection(state) {
    document.querySelectorAll(".tab-content").forEach(section => hide(section));
    show(document.getElementById(sectionIds[state]));
    mainElement.dataset.view = sectionIds[state];
    delete mainElement.dataset.result;
}

// show the in-progress section with only this view visible
export function showRoundView(viewId) {
    showSection(GameState.InProgress);
    document.querySelectorAll(".round-view").forEach(view => hide(view));
    show(document.getElementById(viewId));
    mainElement.dataset.view = viewId; 
}

// call once, onPick(option, index) runs when a button is clicked
export function onAnswerPicked(onPick) {
    answerButtons.forEach((button, index) =>
        button.addEventListener("click", () => onPick(currentOptions[index], index)));
}

// "Q 03 / 10 · " in front of the answering screen's kicker
export function showQuestionLabel(number, count) {
    questionLabelElement.textContent = `${questionLabel(number, count)} · `;
}

// show one button per answer option (letters only), hide the rest
export function renderAnswerButtons(options) {
    currentOptions = options;
    answerButtons.forEach((button, index) => {
        button.hidden = index >= options.length;
    });
}

// show the locked-in screen with a copy of the chosen button
export function showSubmittedAnswer(index) {
    const chosen = answerButtons[index];
    submittedLetterElement.className = `${chosen.className} cg-option-locked`;
    submittedLetterElement.replaceChildren(...chosen.cloneNode(true).childNodes);

    const letter = chosen.querySelector(".cg-option-letter").textContent;
    submittedAnswerElement.textContent = `You picked ${letter}`;
    showRoundView("submitted-view");
}

// green or black panel; numbers are filled in later by showStats/showGain
export function showAnswerResult(text, isCorrect) {
    resultView.classList.toggle("is-correct", isCorrect);
    resultView.classList.toggle("is-wrong", !isCorrect);

    playerAnswerElement.textContent = `You answered: ${text}`;
    answerResultElement.textContent = isCorrect ? "CORRECT" : "NOT QUITE";

    showRoundView("result-view");
    mainElement.dataset.result = isCorrect ? "correct" : "wrong";   
}

// time's up panel, 
export function showTimeUp() {
    showRoundView("time-up-view");
    statTargets.timeUp.score.textContent = "…";
    statTargets.timeUp.place.textContent = "…";
}

// write score + place into one of statTargets
export function showStats(stat, target) {
    target.score.textContent = points(stat.score);
    target.place.textContent = ordinal(stat.place);
}

// big "+850" on the result panel
export function showGain(gain) {
    resultGainElement.textContent = `+${points(gain)}`;
}

// final results: place + score, and the podium badge for the top 3
export function showFinal(stat) {
    showStats(stat, statTargets.finished);

    const podium = stat.place <= 3;
    finalPlaceBox.classList.toggle("is-podium", podium);
    finalPodium.hidden = !podium;
}
