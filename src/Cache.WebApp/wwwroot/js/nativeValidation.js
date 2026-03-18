document.addEventListener("DOMContentLoaded", function() {
    document.querySelectorAll("[data-compare-to]").forEach(function(input) {
        const targetSelector = input.getAttribute("data-compare-to");
        const target = targetSelector ? document.querySelector(targetSelector) : null;

        if (!target) {
            return;
        }

        const syncValidity = function() {
            if (!input.value || input.value === target.value) {
                input.setCustomValidity("");
                return;
            }

            input.setCustomValidity(input.getAttribute("data-compare-message") || "Values do not match.");
        };

        input.addEventListener("input", syncValidity);
        target.addEventListener("input", syncValidity);
    });
});
