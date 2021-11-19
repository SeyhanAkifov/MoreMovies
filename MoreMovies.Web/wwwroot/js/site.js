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

$(document).ready(function () {  

    $(".movie-image").hover(function () {
        $(this).find(".play").show();

    },
        function () {
            $(this).find(".play").hide();
        });


    $(".blink").focus(function () {
        if (this.title == this.value) {
            this.value = '';
        }
    })
        .blur(function () {
            if (this.value == '') {
                this.value = this.title;
            }
        });
});

//Show movie info
var infoButton = document.querySelector("#movieInfo");
infoButton.addEventListener('click', (e) => {
    e.preventDefault();

    var infoDiv = document.querySelector('#info');

    if (infoButton.innerHTML === "Show info") {
        var id = infoButton.value;
        fetch(`https://localhost:5001/api/GetDetails/${id}`, {
            method: 'Get',
            headers: {
                'Content-Type': 'application/json',
            },

        }).then(response => response.json())
            .then(data => {
                console.log(data.title);
                document.querySelector('#language-info').innerHTML = `Language: ${data.language}`;
                document.querySelector('#genre-info').innerHTML = `Genre: ${data.genre}`;
                document.querySelector('#date-info').innerHTML = `Release Date: ${data.releaseDate.toString()}`;
                document.querySelector('#description-info').innerHTML = `Description: ${data.description}`;
                document.querySelector('#country-info').innerHTML = `Country: ${data.country}`;
                document.querySelector('#budget-info').innerHTML = `Budget: ${data.budget} $`;
                document.querySelector('#homepage-info').innerHTML = `Home page: ${data.homePage}`;
            });

        infoDiv.style.display = "block";
        infoButton.innerHTML = "Hide info";
    } else {
        infoDiv.style.display = "none";
        infoButton.innerHTML = "Show info";
    }

});

//Preload logo







