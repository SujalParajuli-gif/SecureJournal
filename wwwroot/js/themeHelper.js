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
