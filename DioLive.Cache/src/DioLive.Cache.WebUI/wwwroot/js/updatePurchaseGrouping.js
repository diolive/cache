$(function () {
    $('.purchase-grouping-slider').change(function () {
        $.post(CFG.updateOptionsUrl, { purchaseGrouping: $(this).val() });
    });
});