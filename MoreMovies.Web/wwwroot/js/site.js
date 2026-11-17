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

const navigationToggle = document.querySelector(".site-nav-toggle");
const navigation = document.getElementById("site-navigation");
const accountMenu = document.querySelector(".site-account");

if (navigationToggle && navigation) {
    navigationToggle.hidden = false;
    navigation.dataset.collapsible = "true";

    function setNavigationExpanded(expanded) {
        navigationToggle.setAttribute("aria-expanded", String(expanded));
        navigation.dataset.expanded = String(expanded);
        if (!expanded && accountMenu) {
            accountMenu.open = false;
        }
    }

    navigationToggle.addEventListener("click", () => {
        setNavigationExpanded(navigationToggle.getAttribute("aria-expanded") !== "true");
    });

    document.addEventListener("keydown", event => {
        if (event.key !== "Escape") {
            return;
        }

        if (accountMenu?.open) {
            accountMenu.open = false;
            accountMenu.querySelector("summary").focus();
        } else if (navigationToggle.getAttribute("aria-expanded") === "true"
            && navigationToggle.getClientRects().length) {
            setNavigationExpanded(false);
            navigationToggle.focus();
        }
    });
}

if (accountMenu) {
    document.addEventListener("click", event => {
        if (!accountMenu.contains(event.target)) {
            accountMenu.open = false;
        }
    });
}

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
