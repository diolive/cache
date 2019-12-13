$(function() {
    $(".category-name").each(function (index, cat) {
        const $cat = $(cat);

        $cat.data("initialName", $cat.val());
    });

    $(".category-name").keyup(function () {
        const $this = $(this);

        if ($this.val() !== $this.data("initialName")) {
            $this.closest("tr").addClass("modified");
        }
    });

    $(".save-changes").click(function (e) {
        var $row = $(this).closest("tr"),
            data = {
                id: $row.data("id"),
                name: $row.find(".category-name").val(),
                color: $row.find(".category-colorpicker").attr("data-color").substring(1),
                parentId: (+$row.find(".category-parent").val()) || null
            },
            $button = $(e.target);

        $button.html(`<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> saving...`);

        postData(CFG.updateCategoryUrl, data)
            .then(() => {
                $row.removeClass("modified");
                $row.find(".category-name").data("initialName", data.name);
            })
            .catch(() => { })
            .then(() => {
                $button.html("save changes");
            });
    });

    $(".category-parent").change(function() {
        $(this).closest("tr").addClass("modified");
    });
});