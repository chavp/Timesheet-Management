Ext.require([
    'Ext.Window',
    'Ext.fx.target.Sprite',
    'Ext.layout.container.Fit',
    'Ext.window.MessageBox',
    'Ext.data.*'
]);

Ext.define('TM.view.ProjectActivitiesBarWindow', {
    extend: 'Ext.window.Window',
    xtype: 'projectactivitiesbarwindow',
    title: 'กราฟแท่ง / Bar Chart',
    resizable: false,
    closable: false,
    autoShow: true,
    constrain: true,
    config: {
        projectData: null
    },

    initComponent: function () {
        var self = this;
        self.projectCode = self.projectData.data.Code;

        self.items = [{
            xtype: 'panel',
            layout: 'fit',
            width: 1200,
            height: 550,
            buttons: [{
                text: messagesForm.closeActionText,
                handler: function () {
                    self.close();
                }
            }],
            items: [{
                xtype: 'box',
                layout: {
                    type: 'fit',
                    padding: 1
                },
                html: '<div class="chart"><svg /></div>'
            }]
        }];

        this.callParent(arguments);
    },

    afterRender: function (component, eOpts) {
        var self = this;
        self.callParent(arguments);
        
        var margin = { top: 20, right: 0, bottom: 60, left: 80 },
                    width = 1150 - margin.left - margin.right,
                    height = 500 - margin.top - margin.bottom;

        var svg = d3.select(".chart svg")
                        .attr("width", width + margin.left + margin.right)
                        .attr("height", height + margin.top + margin.bottom)
                    .append("g")
                        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

        var x = d3.scale.ordinal()
                    .rangeRoundBands([0, width - margin.left - margin.right], .1);

        var y = d3.scale.linear()
                    .range([height, 0]);

        var xAxis = d3.svg.axis()
                        .scale(x)
                        .orient("bottom");
        //xAxis.innerTickSize(-height);

        var yAxis = d3.svg.axis()
                        .scale(y)
                        .orient("left");
        yAxis.innerTickSize(-(width- margin.left - margin.right));


        function render(results) {
            var raws = results.data, 
                roles = results.projectRoles,
                data = [];
            for (var i = 0; i < raws.length; i++) {
                var raw = raws[i];
                var d = {};
                d.date = raw.Date;

                for (var j = 0; j < raw.Efforts.length; j++) {
                    var roleName = roles[j];
                    var effort = raw.Efforts[j];
                    d[roleName] = effort;
                }

                data.push(d);
            }

            data.forEach(function (d) {
                var y0 = 0;
                d.mapping = roles.map(function (name) {
                    return {
                        name: name,
                        label: d["date"],
                        y0: y0,
                        y1: y0 += +d[name]
                    }
                });
                d.total = d.mapping[d.mapping.length - 1].y1;
            });

            x.domain(data.map(function (d) { return d.date; }));
            y.domain([0, d3.max(data, function (d) { return d.total; })]);

            svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + height + ")")
            .call(xAxis)
            .selectAll("text")
                .style("text-anchor", "end")
                .attr("dx", "-.8em")
                .attr("dy", "-.55em")
                .attr("transform", "rotate(-65)");

            svg.append("g")
                .attr("class", "y axis")
                .call(yAxis)
              .append("text")
                .attr("transform", "rotate(-90)")
                .attr("y", 6)
                .attr("dy", ".71em")
                .style("text-anchor", "end")
                .text("Effort (hrs)");

            var colors = d3.scale.category20().domain(roles);

            var series = svg.selectAll(".serie").data(data);

            series.enter().append("g")
                .attr("class", "serie")
                .attr("transform", function (d) { return "translate(" + x(d.date) + ",0)"; });

            var rects = series.selectAll("rect")
              .data(function (d) { return d.mapping; });

            rects.enter().append("rect")
              .attr("width", x.rangeBand())
              .attr("y", function (d) { return y(d.y1); })
              .attr("height", function (d) { return y(d.y0) - y(d.y1); })
              .style("fill", function (d) { return colors(d.name); })
              .style("stroke", "grey");


            var legends = svg.selectAll(".legend")
                .data(roles.slice().reverse());

            legends.exit().remove();

            legends.enter()
                .append("g")
                    .attr("class", "legend")
                    .attr("transform", function (d, i) { return "translate(0," + i * 20 + ")"; });

            legends.append("rect")
                .attr("x", width - 10)
                .attr("width", 10)
                .attr("height", 10)
                .style("fill", colors)
                .style("stroke", "grey");

            legends.append("text")
                .attr("x", width - 12)
                .attr("y", 6)
                .attr("dy", ".35em")
                .style("text-anchor", "end")
                .text(function (d) { return d; });

            //console.log(data);
        }
        
        var url_api = paramsView.urlGetEffortTimesheetItem + "?projectCode=" + self.projectCode;

        self.setLoading(true);
        d3.json(url_api, function (error, data) {
            self.setLoading(false);
            if (error) {
                console.log(error);
            } else {
                //console.log(data);
                render(data);
            }
        });
    }
});