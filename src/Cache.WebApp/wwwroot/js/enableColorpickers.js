$(function() {
    $(".category-colorpicker").colorpicker({
            useAlpha: false
        })
        .on("colorpickerChange",
            function(e) {
                const $this = $(this);

                $this.css("background-color", e.value);
                $this.attr("data-color", e.value.toHexString());
                $this.closest("tr").addClass("modified");
            });
});