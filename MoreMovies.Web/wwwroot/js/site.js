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
