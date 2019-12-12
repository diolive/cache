$(function() {
    $(".category-name").each(function (index, cat) {
        var $cat = $(cat);

        $cat.data("initialName", $cat.val());
    });

    $(".category-name").keyup(function () {
        var $this = $(this);

        if ($this.val() !== $this.data("initialName")) {
            $this.closest("tr").addClass("modified");
        }
    });

    $(".save-changes").click(function() {
        var $row = $(this).closest("tr"),
            data = {
                id: $row.data("id"),
                name: $row.find(".category-name").val(),
                color: $row.find(".colorpicker").data("color").substring(1),
                parentId: $row.find(".category-parent").val()
            };

        $.post(CFG.updateCategoryUrl, data)
            .done(function() {
                $row.removeClass("modified");
                document.location.reload();
            });
    });

    $(".category-parent").change(function() {
        $(this).closest("tr").addClass("modified");
    });
});