$(function () {
    $('.colorpicker').colorpicker({
        format: 'hex'
    })
        .on('changeColor', function (evt) {
            var $this = $(this),
                hexColor = evt.color.toHex();

            this.style.backgroundColor = hexColor;
            $this.closest('tr').addClass('modified');
        });
});