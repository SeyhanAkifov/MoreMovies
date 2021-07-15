// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

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




