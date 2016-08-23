$(function () {
    sunburst('.last-week', CFG.chartsSunburstDataUrl + '?days=7', 480, 350);
    sunburst('.last-month', CFG.chartsSunburstDataUrl + '?days=30', 480, 350);
});