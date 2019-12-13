$(function() {
    $(".locale-switcher").click(function(e) {
        postData(CFG.switchLocaleUrl, { locale: $(e.target).val() })
            .then(() => document.location.reload());
    });
});