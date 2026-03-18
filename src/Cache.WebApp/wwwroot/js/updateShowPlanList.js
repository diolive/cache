$(function() {
    $(".show-plan-list").change(function() {
        postData(CFG.updateOptionsUrl, { showPlanList: $(this).is(":checked") });
    });
});