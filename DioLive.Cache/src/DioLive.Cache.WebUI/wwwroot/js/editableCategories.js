$(function () {
    $('[contenteditable=true]').keyup(function (evt) {
        var $this = $(this),
            isEmpty = $this.hasClass('empty'),
            text = $this.text();

        if (isEmpty && text) {
            $this.removeClass('empty');
        }
        else if (!isEmpty && !text) {
            $this.addClass('empty');
            $this.text($this.siblings().first().text());
        }

        $this.parent().addClass("modified");
    });

    $('.save-changes').click(function () {
        var $row = $(this).closest('tr'),
            $items = $row.children(),
            data = [];

        for (let i = 0; i < $items.length - 1; i++) {
            let $item = $($items[i]);

            data[i] = ($item.hasClass('empty')) ? null : $item.text().trim();
        }

        $.post(CFG.updateCategoryUrl, { id: $row.data('id'), data: data })
            .done(function () {
                $row.removeClass('modified');
            });
    });
});