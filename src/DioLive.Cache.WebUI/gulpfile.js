/// <binding BeforeBuild='default' />
var gulp = require("gulp"),
    rimraf = require("rimraf"),
    less = require("gulp-less");

gulp.task("jquery:core",
    function() {
        return gulp.src("node_modules/jquery/dist/jquery.js")
            .pipe(gulp.dest("wwwroot/js/lib"));
    });

gulp.task("jquery-validation",
    function() {
        return gulp.src("node_modules/jquery-validation/dist/*.js")
            .pipe(gulp.dest("wwwroot/js/lib"));
    });

gulp.task("jquery-validation-unobtrusive",
    function() {
        return gulp.src("node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js")
            .pipe(gulp.dest("wwwroot/js/lib"));
    });

gulp.task("bootstrap:less",
    function() {
        return gulp.src("Styles/bootstrap.less")
            .pipe(less())
            .pipe(gulp.dest("wwwroot/css"));
    });

gulp.task("bootstrap:js",
    function() {
        return gulp.src("node_modules/bootstrap/dist/js/bootstrap.js")
            .pipe(gulp.dest("wwwroot/js/lib"));
    });

gulp.task("bootstrap:fonts",
    function() {
        return gulp.src("node_modules/bootstrap/dist/fonts/*")
            .pipe(gulp.dest("wwwroot/fonts"));
    });

gulp.task("bootstrap3-typeahead",
    function() {
        return gulp.src("node_modules/bootstrap-3-typeahead/bootstrap3-typeahead.js")
            .pipe(gulp.dest("wwwroot/js/lib"));
    });

gulp.task("bootstrap-datepicker:css",
    function() {
        return gulp.src("node_modules/bootstrap-datepicker/dist/css/bootstrap-datepicker3.css")
            .pipe(gulp.dest("wwwroot/css"));
    });

gulp.task("bootstrap-datepicker:js",
    function() {
        return gulp.src("node_modules/bootstrap-datepicker/dist/js/bootstrap-datepicker.js")
            .pipe(gulp.dest("wwwroot/js/lib"));
    });

gulp.task("bootstrap-datepicker:locales",
    function() {
        return gulp.src("node_modules/bootstrap-datepicker/dist/locales/*.js")
            .pipe(gulp.dest("wwwroot/js/lib/locales"));
    });

gulp.task("bootstrap-slider:css",
    function() {
        return gulp.src("node_modules/bootstrap-slider/dist/css/bootstrap-slider.css")
            .pipe(gulp.dest("wwwroot/css"));
    });

gulp.task("bootstrap-slider:js",
    function() {
        return gulp.src("node_modules/bootstrap-slider/dist/bootstrap-slider.js")
            .pipe(gulp.dest("wwwroot/js/lib"));
    });

gulp.task("textarea-autosize",
    function() {
        return gulp.src("node_modules/textarea-autosize/dist/jquery.textarea_autosize.js")
            .pipe(gulp.dest("wwwroot/js/lib"));
    });

gulp.task("site:less",
    function() {
        return gulp.src("Styles/site.less")
            .pipe(less())
            .pipe(gulp.dest("wwwroot/css"));
    });

gulp.task("bootstrap-override",
    function() {
        return gulp.src("Styles/bootstrap-override.less")
            .pipe(less())
            .pipe(gulp.dest("wwwroot/css"));
    });

gulp.task("bootstrap-colorpicker:css",
    function() {
        return gulp.src("node_modules/bootstrap-colorpicker/dist/css/bootstrap-colorpicker.css")
            .pipe(gulp.dest("wwwroot/css"));
    });

gulp.task("bootstrap-colorpicker:img",
    function() {
        return gulp.src("node_modules/bootstrap-colorpicker/dist/img/**/*")
            .pipe(gulp.dest("wwwroot/img"));
    });

gulp.task("bootstrap-colorpicker:js",
    function() {
        return gulp.src("node_modules/bootstrap-colorpicker/dist/js/bootstrap-colorpicker.js")
            .pipe(gulp.dest("wwwroot/js/lib"));
    });

gulp.task("d3:js",
    function() {
        return gulp.src("node_modules/d3/dist/d3.js")
            .pipe(gulp.dest("wwwroot/js/lib"));
    });

gulp.task("d3charts:less",
    function() {
        return gulp.src("Styles/d3charts.less")
            .pipe(less())
            .pipe(gulp.dest("wwwroot/css"));
    });

gulp.task("jquery", gulp.series("jquery:core", "jquery-validation", "jquery-validation-unobtrusive"));

gulp.task("bootstrap:core", gulp.series("bootstrap:less", "bootstrap:js", "bootstrap:fonts"));

gulp.task("bootstrap-datepicker",
    gulp.series("bootstrap-datepicker:css", "bootstrap-datepicker:js", "bootstrap-datepicker:locales"));

gulp.task("bootstrap-slider", gulp.series("bootstrap-slider:css", "bootstrap-slider:js"));

gulp.task("bootstrap-colorpicker",
    gulp.series("bootstrap-colorpicker:css", "bootstrap-colorpicker:img", "bootstrap-colorpicker:js"));

gulp.task("bootstrap",
    gulp.series("bootstrap:core", "bootstrap3-typeahead", "bootstrap-datepicker", "bootstrap-slider", "bootstrap-colorpicker"));

gulp.task("d3", gulp.series("d3:js", "d3charts:less"));

gulp.task("default", gulp.series("jquery", "bootstrap", "textarea-autosize", "site:less", "bootstrap-override", "d3"));