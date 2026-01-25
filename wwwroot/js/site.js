// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const dateSelect = document.getElementById("dateSelect");
const cinemaSelect = document.getElementById("cinemaSelect");
const cards = document.querySelectorAll(".showtime-card");
const container = document.getElementById("showtimeContainer");
const noResult = document.getElementById("noResult");

// Ẩn tất cả khi load trang
container.style.display = "none";
noResult.style.display = "none";

dateSelect.addEventListener("change", () => {
    cinemaSelect.disabled = !dateSelect.value;
    cinemaSelect.value = "";
    hideAll();
});

cinemaSelect.addEventListener("change", filterShowtimes);

function hideAll() {
    cards.forEach(c => c.style.display = "none");
    container.style.display = "none";
    noResult.style.display = "none";
}

function filterShowtimes() {
    const date = dateSelect.value;
    const cinema = cinemaSelect.value;

    // chưa chọn đủ thì không làm gì
    if (!date || !cinema) {
        hideAll();
        return;
    }

    let count = 0;

    cards.forEach(card => {
        const match =
            card.dataset.date === date &&
            card.dataset.cinema === cinema;

        card.style.display = match ? "block" : "none";
        if (match) count++;
    });

    container.style.display = count > 0 ? "flex" : "none";
    noResult.style.display = count === 0 ? "block" : "none";
}
