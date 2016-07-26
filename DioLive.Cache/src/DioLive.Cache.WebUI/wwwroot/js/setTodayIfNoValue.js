$(function () {
    var $date = $('#date'),
        $setToday = $('.set-date-today');

    if (!$date.val()) {
        $setToday.click();
    }
});