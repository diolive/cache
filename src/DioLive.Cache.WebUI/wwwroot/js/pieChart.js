﻿function pie(targetSelector, url, width, height) { // 960-500
    var radius = Math.min(width, height) / 2,
        arc = d3.arc()
            .outerRadius(radius - 10)
            .innerRadius(0),
        labelArc = d3.arc()
            .outerRadius(radius - 40)
            .innerRadius(radius - 40),
        pie = d3.pie()
            .sort(null)
            .value(d => d.totalCost),

        svg = d3.select(targetSelector)
            .append("svg:svg")
                .attr("width", width)
                .attr("height", height)
            .append("g")
                .attr("transform", `translate(${width / 2}, ${height / 2})`);

    d3.json(url, function (error, data) {
        if (error) throw error;

        var g = svg.selectAll(".arc")
            .data(pie(data))
          .enter().append("g")
            .attr("class", "arc");

        g.append("path")
            .attr("d", arc)
            .style("fill", d => '#' + d.data.color);

        g.append("text")
            .attr("transform", function (d) { return "translate(" + labelArc.centroid(d) + ")"; })
            .attr("dy", ".35em")
            .text(d => d.data.displayName);
    });
}