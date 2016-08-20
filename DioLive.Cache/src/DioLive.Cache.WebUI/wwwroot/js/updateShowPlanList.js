$(function () {
    $('.show-plan-list').change(function () {
        $.post(CFG.updateOptionsUrl, { showPlanList: $(this).is(':checked') });
    });
});