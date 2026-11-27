export const API_BASE = "http://localhost:5129";

// get the player's view of the current game or null 
export async function getGame(joinCode, playerId) {
    const res = await fetch(`${API_BASE}/api/Game/${joinCode}/players/${playerId}/game`);
    if (!res.ok) return null;
    return await res.json();
}

// get player place and score or null 
export async function getStat(joinCode, playerId) {
    const res = await fetch(`${API_BASE}/api/Game/${joinCode}/players/${playerId}/stat`);

    if (!res.ok) {
        const err = await res.json().catch(() => null);
        console.error(res.status, err);
        return null;
    }
    return await res.json();
}

// send the answer to the API
export async function sendAnswer(joinCode, playerId, answerId) {
    try {
        const res = await fetch(`${API_BASE}/api/Game/${joinCode}/answer`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ playerId, answerId })
        });

        if (!res.ok) console.error(res.status, await res.text());
    } catch (err) {
        console.error(err);
    }
}
