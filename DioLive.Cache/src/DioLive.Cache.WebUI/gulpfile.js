var gulp = require('gulp'),
    rimraf = require('rimraf'),
    less = require('gulp-less'),

    project = require('./project.json');

gulp.task('jquery:core', function () {
    return gulp.src('bower_components/jquery/dist/jquery.js')
        .pipe(gulp.dest('wwwroot/js/lib'));
});

gulp.task('jquery-validation', function () {
    return gulp.src('bower_components/jquery-validation/dist/*.js')
        .pipe(gulp.dest('wwwroot/js/lib'));
});

gulp.task('jquery-validation-unobtrusive', function () {
    return gulp.src('bower_components/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js')
        .pipe(gulp.dest('wwwroot/js/lib'));
});

gulp.task('bootstrap:less', function () {
    return gulp.src('Styles/bootstrap.less')
        .pipe(less())
        .pipe(gulp.dest('wwwroot/css'));
});

gulp.task('bootstrap:js', function () {
    return gulp.src('bower_components/bootstrap/dist/js/bootstrap.js')
        .pipe(gulp.dest('wwwroot/js/lib'));
});

gulp.task('bootstrap:fonts', function () {
    return gulp.src('bower_components/bootstrap/dist/fonts/*')
        .pipe(gulp.dest('wwwroot/fonts'));
});

gulp.task('bootstrap3-typeahead', function () {
    return gulp.src('bower_components/bootstrap3-typeahead/bootstrap3-typeahead.js')
        .pipe(gulp.dest('wwwroot/js/lib'));
});

gulp.task('bootstrap-datepicker:js', function () {
    return gulp.src('bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.js')
        .pipe(gulp.dest('wwwroot/js/lib'));
});

gulp.task('bootstrap-datepicker:locales', function () {
    return gulp.src('bower_components/bootstrap-datepicker/dist/locales/*.js')
        .pipe(gulp.dest('wwwroot/js/lib/locales'));
});

gulp.task('textarea-autosize', function () {
    return gulp.src('bower_components/textarea-autosize/dist/jquery.textarea_autosize.js')
        .pipe(gulp.dest('wwwroot/js/lib'));
});

gulp.task('jquery', ['jquery:core', 'jquery-validation', 'jquery-validation-unobtrusive']);

gulp.task('bootstrap:core', ['bootstrap:less', 'bootstrap:js', 'bootstrap:fonts']);

gulp.task('bootstrap-datepicker', ['bootstrap-datepicker:js', 'bootstrap-datepicker:locales']);

gulp.task('bootstrap', ['bootstrap:core', 'bootstrap3-typeahead', 'bootstrap-datepicker']);

gulp.task('default', ['jquery', 'bootstrap', 'textarea-autosize']);