$(function() {
    $(document).keydown(function(event) {
        if (event.keyCode === 80) /* P */
        {
            if (document.activeElement.tagName.toUpperCase() !== "INPUT") {
                document.getElementById("addPurchaseButton").click();
            } else {
                event.stopPropagation();
            }
        }
    });
});