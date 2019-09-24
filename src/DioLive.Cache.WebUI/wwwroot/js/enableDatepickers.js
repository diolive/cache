$(function() {
    var options = {
        format: CFG.dateFormat,
        todayHighlight: true,
        autoclose: true
    };
    if (CFG.culture !== "en") {
        options.language = CFG.culture;
    }
    $(".datepicker").datepicker(options);
});