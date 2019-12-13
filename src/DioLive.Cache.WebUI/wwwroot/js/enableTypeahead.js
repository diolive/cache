$(function() {
    const $category = $("#CategoryId");

    $.get(CFG.shopListUrl,
        function(data) {
            $(".typeahead-shops").typeahead({
                source: data
            });
        },
        "json");

    $(".typeahead-names").typeahead({
        source: function(query, process) {
            return $.ajax({
                url: CFG.purchaseListUrl,
                type: "get",
                data: {
                    q: query
                },
                success: function(data) {
                    return process(data);
                }
            });
        },
        autoSelect: false,
        afterSelect: function(item) {
            $.ajax({
                url: CFG.latestCategoryUrl,
                type: "get",
                data: {
                    purchase: item
                },
                success: function(result) {
                    $category.val(result);
                }
            });
        }
    });
});