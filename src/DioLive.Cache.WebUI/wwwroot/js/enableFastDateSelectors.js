﻿$(function() {
    $(".set-date-today").click(function() {
        const date = new Date();

        $(".datepicker").datepicker("setDate", date);
    });

    $(".set-date-yesterday").click(function() {
        const date = new Date();

        date.setDate(date.getDate() - 1);
        $(".datepicker").datepicker("setDate", date);
    });
});