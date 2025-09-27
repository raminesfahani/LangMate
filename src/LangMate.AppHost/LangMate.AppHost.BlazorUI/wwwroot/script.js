window.scrollToBottom = (element) => {
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

window.focusElement = (element) => {
    if (element) {
        element.focus();
    }
};

window.triggerClick = function(element) {
    element.click();
};

window.attachEnterHandler = function(element, dotNetHelper) {
    element.addEventListener("keydown", function(e) {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault(); // ✅ works!
            dotNetHelper.invokeMethodAsync("OnEnterPressed");
        }
    });
};

var currentTheme = "light";
// Function to toggle theme
window.toggleTheme = function() {
    var toggleThemeButton = document.getElementById("toggle-theme");
    if (currentTheme === "light") {
        document.body.classList.add("dark-mode");
        currentTheme = "dark";
        toggleThemeButton.innerHTML =
            '<i class="fas fa-sun"></i><span>Light Mode</span>';
    } else {
        document.body.classList.remove("dark-mode");
        currentTheme = "light";
        toggleThemeButton.innerHTML =
            '<i class="fas fa-moon"></i><span>Dark Mode</span>';
    }
}