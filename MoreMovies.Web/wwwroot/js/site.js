// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

console.log('my js file');
var playButtons = document.querySelectorAll(".movie-image");
var holder = document.querySelector(".holder > button");



playButtons.forEach(button => button.addEventListener("click", (e) => {
    e.preventDefault();
    var trailerUrl = document.querySelector("#trailerUrl").innerHTML;

    var current = document.querySelector(".modal-body > iframe")
    current.src = trailerUrl;
    holder.click();
}));

var videoPlayer = `<div class="col-sm-1">
                    <iframe width="560" height="315" src="https://www.youtube.com/embed/Af2euSk-oeo" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
                </div>
<div class="bg-white shadow rounded p-2">
      <div class="height-400 bg-img-hero-center" style="background-image: url("/css/images/movie1.jpg");">
        <a class="js-fancybox u-media-player u-media-player--centered" href="javascript:;"
           <div class="col-sm-1">
                    <iframe width="560" height="315" src="https://www.youtube.com/embed/Af2euSk-oeo" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
                </div>
        </a>
      </div>
    </div>`


