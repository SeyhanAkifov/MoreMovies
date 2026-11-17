// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Live "new comment" notifications via the /moviehub SignalR hub.
(function setupCommentNotifications() {
    if (typeof signalR === "undefined") {
        return;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/moviehub")
        .build();

    connection.on("NewMessage", (user, movieTitle) => {
        console.log(`${user} commented on ${movieTitle}`);
    });

    connection.start().catch(err => console.error(err.toString()));
})();

const trailerDialog = document.getElementById("trailerModal");
const trailerFrame = document.getElementById("trailerFrame");

if (trailerDialog && trailerFrame) {
    document.querySelectorAll("button[data-trailer]").forEach(button => {
        button.addEventListener("click", () => {
            const trailer = URL.parse(button.dataset.trailer, document.baseURI);
            if (!trailer || !["https:", "http:"].includes(trailer.protocol)) {
                return;
            }

            if (["www.youtube.com", "youtube.com"].includes(trailer.hostname)
                && trailer.pathname === "/watch" && trailer.searchParams.has("v")) {
                trailer.pathname = "/embed/" + encodeURIComponent(trailer.searchParams.get("v"));
                trailer.searchParams.delete("v");
            }

            trailerFrame.src = trailer.href;
            trailerDialog.showModal();
        });
    });

    trailerDialog.addEventListener("close", () => trailerFrame.removeAttribute("src"));
}
