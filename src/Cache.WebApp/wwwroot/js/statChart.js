function stat(targetSelector, url, width, height) {
    const margin = { top: 30, right: 50, bottom: 30, left: 30 };
    const svg = d3.select(targetSelector)
        .append("svg")
        .attr("width", width)
        .attr("height", height);
    var labelPadding = 3,

        g = svg.append("g")
            .attr("transform", `translate(${margin.left},${margin.top})`);

    width -= (margin.left + margin.right);
    height -= (margin.top + margin.bottom);

    d3.json(url).then(function(data) {
        const parseDate = d3.timeParse("%Y-%m-%d");

        for (let i in data.data) {
            data.data[i].date = parseDate(data.data[i].date);
        }

        const series = data.columns.map(function(key, index) {
            return data.data.map(function(d) {
                return {
                    key: key.name,
                    color: `#${key.color}`,
                    date: d.date,
                    value: d.values[index] | 0
                };
            });
        });

        var x = d3.scaleTime()
            .domain([data.data[0].date, data.data[data.data.length - 1].date])
            .range([0, width]);

        var y = d3.scaleLinear()
            .domain([0, d3.max(series, function(s) { return d3.max(s, function(d) { return d.value; }); })])
            .range([height, 0]);

        const z = d3.scaleOrdinal(d3.schemeCategory10);

        g.append("g")
            .attr("class", "axis axis--x")
            .attr("transform", `translate(0,${height})`)
            .call(d3.axisBottom(x));

        const serie = g.selectAll(".serie")
            .data(series)
            .enter().append("g")
            .attr("class", "serie");

        serie.append("path")
            .attr("class", "line")
            .style("stroke", function(d) { return d[0].color; })
            .attr("d",
                d3.line()
                .x(function(d) { return x(d.date); })
                .y(function(d) { return y(d.value); }));

        const label = serie.selectAll(".label")
            .data(function(d) { return d; })
            .enter().append("g")
            .attr("class", "label")
            .attr("transform", function(d, i) { return `translate(${x(d.date)},${y(d.value)})`; });

        const rect = label.append("rect");

        label.append("text")
            .attr("dy", ".35em")
            .text(function(d) { return d.value; })
            .filter(function(d, i) { return i === data.data.length - 1; })
            .append("tspan")
            .attr("class", "label-key")
            .text(function(d) { return ` ${d.key}`; });

        rect
            .datum(function() { return this.nextSibling.getBBox(); })
            .attr("x", function(d) { return d.x - labelPadding; })
            .attr("y", function(d) { return d.y - labelPadding; })
            .attr("width", function(d) { return d.width + 2 * labelPadding; })
            .attr("height", function(d) { return d.height + 2 * labelPadding; });
    });
}