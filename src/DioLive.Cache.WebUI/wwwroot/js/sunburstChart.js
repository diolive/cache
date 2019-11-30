function sunburst(targetSelector, url, width, height, currency) { // 960-700
    var radius = Math.min(width, height) / 2 - 10,
        formatNumber = d3.format(",d"),

        x = d3.scaleLinear().range([0, 2 * Math.PI]),
        y = d3.scaleSqrt().range([0, radius]),

        partition = d3.partition(),
        arc = d3.arc()
            .startAngle(d => Math.max(0, Math.min(2 * Math.PI, x(d.x0))))
            .endAngle(d => Math.max(0, Math.min(2 * Math.PI, x(d.x1))))
            .innerRadius(d => Math.max(0, y(d.y0)))
            .outerRadius(d => Math.max(0, y(d.y1))),

        svg = d3.select(targetSelector)
            .append("svg")
            .attr("width", width)
            .attr("height", height)
            .append("g")
            .attr("transform", `translate(${width / 2}, ${height / 2})`);

    d3.json(url).then(function(root) {
        root = d3.hierarchy(root);
        root.sum(d => d.totalCost);
        svg.selectAll("path")
            .data(partition(root).descendants())
            .enter().append("path")
            .attr("d", arc)
            .style("fill", d => "#" + d.data.color)
            .on("click", click)
            .append("title")
            .text(d => `${d.data.displayName}\n${formatNumber(d.value)} ${currency}`);
    });

    function click(d) {
        svg.transition()
            .duration(750)
            .tween("scale",
                function() {
                    var xd = d3.interpolate(x.domain(), [d.x0, d.x1]),
                        yd = d3.interpolate(y.domain(), [d.y0, 1]),
                        yr = d3.interpolate(y.range(), [d.y0 ? 20 : 0, radius]);
                    return function(t) {
                        x.domain(xd(t));
                        y.domain(yd(t)).range(yr(t));
                    };
                })
            .selectAll("path")
            .attrTween("d", d => () => arc(d));
    }

    d3.select(self.frameElement).style("height", height + "px");
}