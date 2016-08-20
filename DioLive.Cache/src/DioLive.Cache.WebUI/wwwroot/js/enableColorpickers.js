$(function () {
    $('.colorpicker').colorpicker({
        input: 'input[type="hidden"]',
        format: 'hex'
    }).on('changeColor', function (evt) {
        $(this).colorpicker('disable');
        $.post(CFG.updateCategoryUrl, { id: $(evt.target).data('id'), color: evt.color.toHex().substring(1) })
            .always(function () {
                $(this).colorpicker('enable');
            });
    });
});