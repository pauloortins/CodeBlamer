var RepoPackView = Class.extend({
    init: function() {
        var self = this;

        self.width = 1280;
        self.height = 800;
        self.ratio = 720;
        self.posX = d3.scale.linear().range([0, self.ratio]);
        self.posY = d3.scale.linear().range([0, self.ratio]);
        
        self.pack = d3.layout.pack()
            .size([self.ratio, self.ratio])
            .value(function (d) { return d.value; });
        
        self.vis = d3.select("body").insert("svg:svg", "h2")
            .attr("width", self.width)
            .attr("height", self.height)
            .append("svg:g")
            .attr("transform", "translate(" + (self.width - self.ratio) / 2 + "," + (self.height - self.ratio) / 2 + ")");

        self._loadData();
    },
    
    _loadData: function () {
        var self = this;

        d3.json("/Repository/GraphJson", function (data) {
            self.node = self.root = data;

            var nodes = self.pack.nodes(self.root);

            self.vis.selectAll("circle")
                .data(nodes)
                .enter().append("svg:circle")
                .attr("class", function (d) { return d.children ? "parent" : "child"; })
                .attr("cx", function (d) { return d.x; })
                .attr("cy", function (d) { return d.y; })
                .attr("r", function (d) { return d.r; })
                .style("fill", self.color)
                //.attr("title", function (d) { return d.name; })
                .on("click", function (d) { return self._zoom(self.node == d ? self.root : d); });

            self.vis.selectAll("circle")
                .data(nodes)
                .append("svg:title").text(function (d) { return d.name; });

            self.vis.selectAll("text")
                .data(nodes)
                .enter().append("svg:text")
                .attr("class", function (d) { return d.children ? "parent" : "child"; })
                .attr("x", function (d) { return d.x; })
                .attr("y", function (d) { return d.y; })
                .attr("dy", ".35em")
                .attr("text-anchor", "middle")
                .style("opacity", function (d) { return d.r > 20 ? 1 : 0; })
                .text(function (d) { return d.name; });


            d3.select(window).on("click", function () { self._zoom(self.root); });
        });
    },
    
    _zoom: function (d, i) {
        var self = this;

        var k = self.ratio / d.r / 2;
        self.posX.domain([d.x - d.r, d.x + d.r]);
        self.posY.domain([d.y - d.r, d.y + d.r]);

        var t = self.vis.transition()
            .duration(d3.event.altKey ? 7500 : 750);

        t.selectAll("circle")
            .attr("cx", function (d) { return self.posX(d.x); })
            .attr("cy", function (d) { return self.posY(d.y); })
            .attr("r", function(d) { return k * d.r; });

        t.selectAll("text")
            .attr("x", function (d) { return self.posX(d.x); })
            .attr("y", function (d) { return self.posY(d.y); })
            .style("opacity", function(d) { return k * d.r > 20 ? 1 : 0; });

        self.node = d;
        d3.event.stopPropagation();
    },
    
    color: function (d) {
        var colors = [
            "#ff0000",
            "#ea1500",
            "#d42b00",
            "#bf4000",
            "#aa5500",
            "#956a00",
            "#808000",
            "#6a9500",
            "#55aa00",
            "#40bf00",
            "#2bd400",
            "#15ea00",
            "#00ff00"
        ];

        var maintainabilityIndex = d.maintainabilityIndex;

        if (maintainabilityIndex == 0)
            return "#fff";

        var colorRange = Math.floor(maintainabilityIndex / 8);
        return colors[colorRange];
    }
});