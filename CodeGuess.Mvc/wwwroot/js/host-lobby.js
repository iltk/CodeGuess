const joinCode = document.getElementById("lobby").dataset.joinCode;
const list = document.getElementById("member-list");


//The URL is the hub's address:
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5129/hubs/game")
    .build();

//response function when the server sends "PlayerJoined"
connection.on("PlayerJoined", nickname => {
    const li = document.createElement("li");
    li.textContent = nickname;
    list.appendChild(li);
});


//Connect, then join this room's group.
connection.start()
    .then(() => connection.invoke("JoinGame", joinCode))
    .catch(err => console.error(err));
