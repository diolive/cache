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

        $this.closest('tr').addClass("modified");
    });

    $('.save-changes').click(function () {
        var $row = $(this).closest('tr'),
            $items = $row.children(),
            data = {
                id: $row.data('id'),
                translates: [],
                color: $row.find('.colorpicker').data('color').substring(1),
                parentId: $row.find('.category-parent').val()
            };

        for (let i = 0; i < $items.length - 1; i++) {
            let $item = $($items[i + 1]);

            data.translates[i] = ($item.hasClass('empty')) ? null : $item.text().trim();
        }

        $.post(CFG.updateCategoryUrl, data)
            .done(function () {
                $row.removeClass('modified');
                document.location.reload();
            });
    });

    $('.category-parent').change(function () {
        $(this).closest('tr').addClass('modified');
    });
});