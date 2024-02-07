"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub", {
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.ServerSentEvents
    })
    .withAutomaticReconnect()
    .build();
let groupName = "group1";

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;
document.getElementById("joinGroupButton").disabled = true;
document.getElementById("pingGroupButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);

    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

connection.on("PingHandler", (message) => {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);

    li.textContent = `Group: '${groupName}' says ${message}`;
});

connection.start().then(function () {

    // enable buttons
    document.getElementById("sendButton").disabled = false;
    document.getElementById("joinGroupButton").disabled = false;
    document.getElementById("pingGroupButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("joinGroupButton").addEventListener("click", function (event) {
    connection.invoke("JoinGroup", groupName).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("joinGroupButton").disabled = true;
    event.preventDefault();
});

document.getElementById("pingGroupButton").addEventListener("click", function (event) {

    var message = document.getElementById("messageInput").value;
    connection.invoke("PingGroup", groupName, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});