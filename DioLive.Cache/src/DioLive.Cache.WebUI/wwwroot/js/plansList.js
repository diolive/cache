$(function () {
    var $plans = $('.plans'),
        $plansList = $('ul', $plans),
        $addPlanBtn = $('.add-plan-btn', $plans),
        $addPlanName = $('.add-plan-name', $plans),
        $deleteBtn = $('button.delete', $plansList);

    $addPlanBtn.click(function () {
        var newItem = $addPlanName.val();

        if (newItem) {
            $.ajax({
                url: CFG.addPlanUrl,
                type: 'post',
                data: {
                    name: newItem
                },
                success: function (response) {
                    var newItem = $(`<li data-id="${response.id}"><button type="button" class="btn btn-danger btn-inline btn-xs pull-right delete" aria-label="Close"><span aria-hidden="true">&times;</span></button> ${response.name}</li>`);
                    newItem.appendTo($plansList);
                    $addPlanName.val('');
                }
            });
        }
    });

    $plansList.on('click', 'li:not(.bought)', function () {
        document.location = CFG.addPurchaseUrl + '?planId=' + $(this).data('id');
    });

    $plansList.on('click', '.delete', function (event) {
        var $li = $(this).closest('li');

        event.stopPropagation();
        $.ajax({
            url: CFG.removePlanUrl,
            type: 'post',
            data: {
                id: $li.data('id')
            },
            success: function () {
                $li.remove();
            }
        });
    });

    $addPlanName.keydown(function (event) {
        event.stopPropagation();
    });
});