$(function () {
    var $removeBudgetName = $("#removeBudgetName"),
        $removeBudgetButton = $("#removeBudgetButton"),
        $removeBudgetModal = $("#removeBudgetModal"),
        $removeBudgetConfirmedButton = $("#removeBudgetConfirmedButton");

    $removeBudgetName.keyup(() => {
        if ($removeBudgetName.val() === $removeBudgetName.data("budgetName")) {
            $removeBudgetButton.removeClass("disabled");
        }
        else {
            $removeBudgetButton.addClass("disabled");
        }
    });

    $removeBudgetConfirmedButton.click(() => {
        $removeBudgetConfirmedButton.html(`<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> removing...`);

        postData(CFG.removeCurrentBudgetUrl)
            .then(() => {
                $removeBudgetModal.modal('hide');
                document.location = CFG.homePageUrl;
            })
            .catch(e => {
                $removeBudgetModal.find(".errorMessage").text(e);
            })
            .then(() => {
                $button.html("Yes, remove this budget");
            });
    });
});