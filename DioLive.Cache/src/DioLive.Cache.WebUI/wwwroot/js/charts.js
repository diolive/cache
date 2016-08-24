$(function () {
    sunburst('#lastWeekChart', CFG.chartsSunburstDataUrl + '?days=7', 480, 350);
    sunburst('#lastMonthChart', CFG.chartsSunburstDataUrl + '?days=30', 480, 350);
    sunburst('#totalChart', CFG.chartsSunburstDataUrl, 480, 350);
});