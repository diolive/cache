$(function() {
    sunburst("#weekChart", CFG.chartsSunburstDataUrl + "?days=7", 480, 350, CFG.currency);
    sunburst("#monthChart", CFG.chartsSunburstDataUrl + "?days=30", 480, 350, CFG.currency);
    sunburst("#months3Chart", CFG.chartsSunburstDataUrl + "?days=92", 480, 350, CFG.currency);
    sunburst("#months6Chart", CFG.chartsSunburstDataUrl + "?days=183", 480, 350, CFG.currency);
    sunburst("#yearChart", CFG.chartsSunburstDataUrl + "?days=365", 480, 350, CFG.currency);
    sunburst("#totalChart", CFG.chartsSunburstDataUrl, 480, 350, CFG.currency);
    stat("#statChart", CFG.chartsStatDataUrl + "?days=10&depth=30&step=3", 960, 700);
});