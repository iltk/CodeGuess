const joinCode = document.getElementById("lobby").dataset.joinCode;
const list = document.getElementById("member-list");
const startButton = document.getElementById("start-button");
const memberCount = document.getElementById("member-count");


//The URL is the hub's address:
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5129/hubs/game")
    .build();

//response function when the server sends "PlayerJoined"
connection.on("PlayerJoined", nickname => {
    const li = document.createElement("li");
    li.textContent = nickname;
    list.appendChild(li);
    startButton.disabled = false;
    const currentCount = memberCount.textContent;
    memberCount.textContent = String(Number(currentCount) + 1);

});




//Connect, then join this room's group.
connection.start()
    .then(() => connection.invoke("JoinGame", joinCode))
    .catch(err => console.error(err));
