// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var logID = 'log',
    log = $('<div id="' + logID + '"></div>');
$('body').append(log);
$('[type*="radio"]').change(function () {
    var me = $(this);
    log.html(me.attr('value'));
});

//play trailer in pop up doalog

console.log('my js file');
var playButtons = document.querySelectorAll(".movie-image");
var holder = document.querySelector(".holder > button");



playButtons.forEach(button => button.addEventListener("click", (e) => {
    e.preventDefault();
    var trailerUrl = e.currentTarget.querySelector("#trailerUrl").innerHTML;

    var current = document.querySelector(".modal-body > iframe")
    current.src = trailerUrl;
    holder.click();
}));

//try SignalR notification on add new comment


setupConnection = () => {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/moviehub")
        .build();

    connection.on("RecieveMessage", (update) => {
        document.getElementById("status").innerHTML = update;
    })

    connection.on("NewMessage", (user, movie) => {
        document.getElementById("status").innerHTML = `${user} commented on this ${movie}`;
    })

    connection.on("Finished", function () {
        connection.stop();
    })

    connection.start()
        .catch(err => console.error(err.toString()))
}

setupConnection();




