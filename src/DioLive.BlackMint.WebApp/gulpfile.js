var gulp = require("gulp");

gulp.task("jquery",
    function() {
        return gulp
            .src("bower_components/jquery/dist/jquery.min.js")
            .pipe(gulp.dest("wwwroot/lib"));
    });

gulp.task("semantic-ui",
    function() {
        return gulp
            .src("bower_components/semantic-ui/dist/semantic.min.*")
            .pipe(gulp.dest("wwwroot/lib"));
    });

gulp.task("semantic-ui:themes",
    function() {
        return gulp
            .src("bower_components/semantic-ui/dist/themes/**/*")
            .pipe(gulp.dest("wwwroot/lib/themes"));
    }
);

gulp.task("default", ["jquery", "semantic-ui", "semantic-ui:themes"]);