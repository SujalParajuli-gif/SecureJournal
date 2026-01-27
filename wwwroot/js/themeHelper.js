// We used this helper to toggle theme-dark on the body
window.themeHelper = {

    setDark: function (isDark) {
        if (isDark) {
            document.body.classList.add("theme-dark");
        } else {
            document.body.classList.remove("theme-dark");
        }
    }
};
// ui helpers for smooth scrolling + temporary highlight
window.uiHelpers = window.uiHelpers || {};

window.uiHelpers.scrollToId = (id) => {
    const el = document.getElementById(id);
    if (!el) return;

    el.scrollIntoView({ behavior: "smooth", block: "start" });
};

window.uiHelpers.flashHighlight = (id) => {
    const el = document.getElementById(id);
    if (!el) return;

    el.classList.add("today-highlight");
    setTimeout(() => el.classList.remove("today-highlight"), 1600);
};
