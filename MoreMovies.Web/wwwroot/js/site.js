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

const seatForm = document.querySelector("[data-seat-booking]");
const reserveButton = seatForm?.querySelector('button[type="submit"]');
if (reserveButton && typeof fetch === "function") {
    const seatStatus = seatForm.querySelector("[data-seat-status]");

    // Submit all selected seats and keep the available selection after conflicts.
    seatForm.addEventListener("submit", async event => {
        event.preventDefault();
        if (reserveButton.disabled) {
            return;
        }

        const selection = new FormData(seatForm);
        const count = selection.getAll("seats").length;
        if (count < 1 || count > 8) {
            seatStatus.textContent = "Select between one and eight seats.";
            return;
        }

        reserveButton.disabled = true;
        seatStatus.textContent = "Reserving seats…";
        try {
            const response = await fetch(seatForm.action, {
                method: "POST",
                headers: { Accept: "application/json" },
                body: selection,
                redirect: "manual"
            });
            if (response.status === 409 && response.headers.get("Content-Type")?.includes("application/json")) {
                const { unavailable, url } = await response.json();
                if (url) {
                    window.location.assign(url);
                    return;
                }

                const blocked = new Set(unavailable ?? []);
                seatForm.querySelectorAll('input[name="seats"]').forEach(input => {
                    input.disabled = blocked.has(Number(input.value));
                    if (input.disabled) {
                        input.checked = false;
                    }
                });
                seatStatus.textContent = "Availability changed. Unavailable seats have been marked. Your other selections are still checked.";
            } else if (response.status === 409) {
                seatStatus.textContent = "Reservations for this screening are closed.";
            } else if (response.ok) {
                const { url } = await response.json();
                window.location.assign(url);
            } else if (response.type === "opaqueredirect" || response.status === 401) {
                seatStatus.textContent = "Sign in again to reserve seats.";
            } else {
                seatStatus.textContent = "Could not reserve these seats. Refresh the plan and try again.";
            }
        } catch {
            seatStatus.textContent = "Could not confirm the reservation. Refresh the plan to check your holds.";
        } finally {
            reserveButton.disabled = false;
        }
    });
}

const holdCountdowns = document.querySelectorAll("[data-hold-countdown]");
if (holdCountdowns.length) {
    // Show the time remaining from the reservation's server-issued expiry.
    function updateHoldCountdowns() {
        holdCountdowns.forEach(countdown => {
            const seconds = Math.max(0, Math.ceil((Date.parse(countdown.dataset.holdCountdown) - Date.now()) / 1000));
            const minutes = Math.floor(seconds / 60);
            const remainder = String(seconds % 60).padStart(2, "0");
            countdown.textContent = seconds > 0 ? `(${minutes}:${remainder} left)` : "(expired)";
        });
    }

    updateHoldCountdowns();
    setInterval(updateHoldCountdowns, 1000);
}

const ratingForm = document.querySelector(".movie-rating-form");
const ratingSummary = document.querySelector("[data-rating-summary]");
const ratingStatus = document.querySelector("[data-rating-status]");

if (ratingForm && ratingSummary && ratingStatus && typeof fetch === "function") {
    const submitButton = ratingForm.querySelector('button[type="submit"]');
    const averageStars = document.querySelectorAll("[data-average-star]");
    let saving = false;

    ratingForm.addEventListener("submit", async event => {
        event.preventDefault();
        if (saving) {
            return;
        }

        saving = true;
        submitButton.setAttribute("aria-disabled", "true");
        ratingStatus.dataset.error = "false";
        ratingStatus.textContent = "Saving rating…";

        try {
            const response = await fetch(ratingForm.action, {
                method: "POST",
                headers: { Accept: "application/json" },
                body: new FormData(ratingForm),
                redirect: "manual"
            });

            if (response.type === "opaqueredirect" || response.status === 401 || response.status === 403) {
                ratingStatus.dataset.error = "true";
                ratingStatus.textContent = "Sign in again to submit your rating.";
                return;
            }

            if (!response.ok || !response.headers.get("Content-Type")?.includes("application/json")) {
                throw new Error("Rating response could not be confirmed.");
            }

            const { average, count } = await response.json();
            const summary = `${average} / 5, ${count} ${count === 1 ? "vote" : "votes"}`;
            ratingSummary.textContent = `(${summary})`;
            averageStars.forEach(star => {
                const filled = Number(star.dataset.averageStar) <= average;
                star.classList.toggle("text-yellow-400", filled);
                star.classList.toggle("text-gray-400", !filled);
            });
            ratingForm.reset();
            ratingStatus.textContent = `Rating saved. ${summary}.`;
        } catch {
            ratingStatus.dataset.error = "true";
            ratingStatus.textContent = "Could not confirm your rating. Refresh to check whether it was saved.";
        } finally {
            saving = false;
            submitButton.removeAttribute("aria-disabled");
        }
    });
}
