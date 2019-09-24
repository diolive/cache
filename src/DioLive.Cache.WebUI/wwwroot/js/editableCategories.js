$(function() {
    $("[contenteditable=true]").keyup(function(evt) {
        var $this = $(this),
            isEmpty = $this.hasClass("empty"),
            text = $this.text();

        if (isEmpty && text) {
            $this.removeClass("empty");
        } else if (!isEmpty && !text) {
            $this.addClass("empty");
            $this.text($this.siblings().first().text());
        }

        $this.closest("tr").addClass("modified");
    });

    $(".save-changes").click(function() {
        var $row = $(this).closest("tr"),
            $items = $row.children(),
            data = {
                id: $row.data("id"),
                translates: $items.filter("[contenteditable=true]").map(function() {
                    var $item = $(this);
                    return ($item.hasClass("empty")) ? "" : $item.text().trim();
                }).get(),
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