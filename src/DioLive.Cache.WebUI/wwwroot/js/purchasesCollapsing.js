$(function() {
    $("body").on("click.collapse-next.data-api",
        "[data-toggle=collapse-next]",
        function() {
            var $this = $(this),
                $target = $this.nextUntil(":not(.collapse)");

            $target.collapse("toggle");
            $this.toggleClass("expanded");
        });
});