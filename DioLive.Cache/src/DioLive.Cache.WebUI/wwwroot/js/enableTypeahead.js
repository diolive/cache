$(function () {
    $.get(CFG.shopListUrl, function (data) {
        $('.typeahead-shops').typeahead({
            source: data
        });
    }, 'json');

    $('.typeahead-names').typeahead({
        source: function (query, process) {
            return $.get(CFG.purchaseListUrl + '?q=' + query, function (data) {
                return process(data);
            });
        }
    });
});