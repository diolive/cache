$(function() {
    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function(element) {
        bootstrap.Tooltip.getOrCreateInstance(element);
    });
});
