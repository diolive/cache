$(function () {
    $(document).keydown(function (event) {
        if (event.keyCode === 80 /* P */) {
            document.getElementById('addPurchaseButton').click();
        }
    });
})