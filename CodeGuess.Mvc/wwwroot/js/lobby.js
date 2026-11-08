const code = document.getElementById("lobby").dataset.code;
const list = document.getElementById("member-list");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5129/hubs/lobby")
    .build();

connection.on("MembersChanged", nickname => {
    const li = document.createElement("li");
    li.textContent = nickname;
    list.appendChild(li);
});

connection.start()
    .then(() => connection.invoke("JoinRoom", code))
    .catch(err => console.error(err));
