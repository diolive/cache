var gulp = require('gulp'),
    rimraf = require('rimraf'),
    less = require('gulp-less'),

    project = require('./project.json');

gulp.task('bootstrap:less', function () {
    return gulp.src('Styles/bootstrap.less')
        .pipe(less())
        .pipe(gulp.dest('wwwroot/css'));
});

gulp.task('bootstrap:js', function () {
    return gulp.src('bower_components/bootstrap/dist/js/bootstrap.js')
        .pipe(gulp.dest('wwwroot/js'));
});

gulp.task('bootstrap:fonts', function () {
    return gulp.src('bower_components/bootstrap/dist/fonts/*')
        .pipe(gulp.dest('wwwroot/fonts'));
});

gulp.task('bootstrap', ['bootstrap:less', 'bootstrap:js', 'bootstrap:fonts']);