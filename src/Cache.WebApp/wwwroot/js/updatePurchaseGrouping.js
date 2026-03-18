$(function() {
    $(".purchase-grouping-slider").change(function() {
        postData(CFG.updateOptionsUrl, { purchaseGrouping: (+$(this).val()) || null });
    });
});