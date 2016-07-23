$(function () {
    $.get(CFG.shopListUrl, function (data) {
        $('.typeahead-shops').typeahead({
            source: data
        });
    }, 'json');
});