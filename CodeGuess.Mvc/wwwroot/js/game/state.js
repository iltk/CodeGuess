// Shared constants and variables for the player game page

// matches GameState enum in the API
export const GameState = { Lobby: 0, InProgress: 1, Finished: 2 };

// Values that change while the game runs.
// Kept in one object because other modules can't reassign an imported `let`,
// but they can change a property: state.newestCallId++
export const state = {
    answerRequest: null,   // the answer being sent to the server , promise
    newestCallId: 0,       // id of the newest showCurrentGame call, to track if the newer avaible from server
};
