Ext.require([
    'Ext.Window',
    'Ext.fx.target.Sprite',
    'Ext.layout.container.Fit',
    'Ext.window.MessageBox',
    'Ext.data.*'
]);

Ext.define('TM.view.ChartWindow', {
    extend: 'Ext.window.Window',
    xtype: 'chartWindow',
    title: 'กราฟ / Chart',
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

        self.callParent(arguments);
    },

    afterRender: function (component, eOpts) {
        this.callParent(arguments);
        var self = this;
        
        var margin = { top: 20, right: 0, bottom: 70, left: 80 },
                              width = 1150 - margin.left - margin.right,
                              height = 500 - margin.top - margin.bottom;

        var svg = d3.select(".chart svg")
                        .attr("width", width + margin.left + margin.right)
                        .attr("height", height + margin.top + margin.bottom)
                    .append("g")
                        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

        function renderCfd(data) {
            var xScale = d3.time.scale()
                        //.domain([new Date("2015-01-1"), d3.time.day.offset(new Date("2015-12-30"), 1)])
                        .rangeRound([0, width - margin.left - margin.right]);

            var parseDate = d3.time.format('%d/%m/%Y').parse;

            if (data.startDate !== "") {

                
            }

            var yScale = d3.scale.linear()
                .domain([0, parseInt(data.total)])
                .rangeRound([height, 0]);

            var yAxis = d3.svg.axis()
                    .scale(yScale)
                    .orient("left");

            svg.append("g")
               .attr("class", "y axis")
               .call(yAxis)
             .append("text")
               //.attr("transform", "rotate(0)")
                .attr('transform', 'translate(' + 30 + ', -20)')
               .attr("y", 6)
               .attr("dy", ".71em")
               .style("text-anchor", "end")
               .text("Cost");

            var maxDate = d3.max(data.data, function (d) { return new Date(d.Date); });
            var minDate = d3.min(data.data, function (d) { return new Date(d.Date); });

            xScale.domain([minDate, d3.time.day.offset(maxDate, 1)]);
            //xScale.domain([new Date(minDate), new Date(maxDate)]);

            var xAxis = d3.svg.axis()
                    .scale(xScale)
                    .orient('bottom')
                    .ticks(d3.time.days, 4)
                    .tickFormat(d3.time.format('%d/%m/%Y'))
                    .tickSize(5);

            svg.append('g')
            .attr('class', 'x axis')
            .attr('transform', 'translate(0, ' + height + ')')
            .call(xAxis);

            var presale_line = d3.svg.line()
                 .x(function (d) { return xScale(new Date(d.Date)); })
                 .y(function (d) { return yScale(d.PreSale); })
                 .interpolate('basis');
            svg.append('g')
                .datum(data.data)
              .append('path')
                .attr('class', 'line-presale')
                .attr('d', presale_line);

            var implementAndDev_line = d3.svg.line()
                 .x(function (d) { return xScale(new Date(d.Date)); })
                 .y(function (d) { return yScale(d.ImplementAndDev); })
                 .interpolate('basis');
            svg.append('g')
                .datum(data.data)
              .append('path')
                .attr('class', 'line-implement')
                .attr('d', implementAndDev_line);

            var warranty_line = d3.svg.line()
                 .x(function (d) { return xScale(new Date(d.Date)); })
                 .y(function (d) { return yScale(d.Warranty); })
                 .interpolate('basis');
            svg.append('g')
                .datum(data.data)
              .append('path')
                .attr('class', 'line-arranty')
                .attr('d', warranty_line);

            var operation_line = d3.svg.line()
                 .x(function (d) { return xScale(new Date(d.Date)); })
                 .y(function (d) { return yScale(d.Operation); })
                 .interpolate('basis');
            svg.append('g')
                .datum(data.data)
              .append('path')
                .attr('class', 'line-operation')
                .attr('d', operation_line);
        }

        function renderCfd2(result) {

            var data = result.data;
            var x = d3.scale.ordinal().rangeRoundBands([0, width], .1);

            //var maxDate = d3.max(data.data, function (d) { return new Date(d.Date); });
            //var minDate = d3.min(data.data, function (d) { return new Date(d.Date); });

            //x.domain([minDate, d3.time.day.offset(maxDate, 1)]);

            var y = d3.scale.linear().rangeRound([height, 0]);

            var xAxis = d3.svg.axis().scale(x).orient("bottom");
            xAxis.innerTickSize(-height);

            var yAxis = d3.svg.axis().scale(y).orient("left");
            yAxis.innerTickSize(-width);

            var stack = d3.layout.stack()
                          .values(function (d) { return d.values; })
                          .x(function (d) { return x(d.label) + x.rangeBand() / 2; })
                          .y(function (d) { return d.value; }).offset("zero");

            var area = d3.svg.area()
                  //.interpolate("cardinal")
                  .x(function (d) { return x(d.label) + x.rangeBand() / 2; })
                  .y0(function (d) { return y(d.y0); })
                  .y1(function (d) { return y(d.y0 + d.y); });

            var phases = ["PreSale", "Implement", "Warranty", "Operation"];
            var colors = d3.scale.category10().domain(phases);

            var seriesArr = [], series = {};
            phases.forEach(function (name) {
                series[name] = { name: name, values: [] };
                seriesArr.push(series[name]);
            });

            data.forEach(function (d) {
                phases.map(function (name) {
                    series[name].values.push({ name: name, label: d["Date"], value: d[name] });
                });
            });

            var date_timesheet = data.map(function (d) { return d.Date; });
            //for (var i = 10; i < 30; i++) {
            //    date_timesheet.push(i + "-03-2015");
            //}
            x.domain(date_timesheet);

            xAxis.ticks(5);

            stack(seriesArr);

            var maxCost = d3.max(seriesArr, function (c) {
                return d3.max(c.values, function (d) { return d.y0 + d.y; });
            });

            // draw line
            var estimatedMarketValue = 100000 - maxCost;
            var projectValue = 80000;
            var presale_lv1 = 0.05 * estimatedMarketValue;
            var presale_lv2 = 0.10 * estimatedMarketValue;

            y.domain([0, maxCost]);
            
            //end

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
            .text("Cost");

            var selection = svg.selectAll(".series")
          .data(seriesArr)
          .enter().append("g")
            .attr("class", "series");

            selection.append("path")
              .attr("class", "streamPath")
              .attr("d", function (d) { return area(d.values); })
              .style("fill", function (d) { return colors(d.name); })
              .style("stroke", "grey");

          //  var points = svg.selectAll(".seriesPoints")
          //.data(seriesArr)
          //.enter().append("g")
          //  .attr("class", "seriesPoints");

            var points = svg.selectAll(".seriesPoints")
                            .data(seriesArr, function (d) { return d.values; });

            points.enter().append("g").attr("class", "seriesPoints");

            //console.log(seriesArr);
            points.selectAll(".point")
                      //.data(function (d) { return d.values; })
                      //.enter()
                      .append("circle")
                       .attr("class", "point")
                       .attr("cx", function (d) { return x(d.label) + x.rangeBand() / 2; })
                       .attr("cy", function (d) { return y(d.y0 + d.y); })
                       .attr("r", "5px")
                       .style("fill", function (d) {
                           return color(d.name);
                       });

            // draw line
            //var lines_data = [
            //    { label: "Estimated Market Value", value: estimatedMarketValue, color: "#0000FF" },
            //    { label: "Project Value", value: projectValue, color: "#007A00" },
            //    { label: "Presale Threshold LV-1", value: presale_lv1, color: "#8F8F00" },
            //    { label: "Presale Threshold LV-2", value: presale_lv2, color: "red" }
            //];

            //var lines = svg.append("g").selectAll("line").data(lines_data);
            //lines.enter()
            //    .append("line")
            //        .attr("x1", 0)
            //        .attr("y1", function (d) { return y(d.value); })
            //        .attr("x2", width)
            //        .attr("y2", function (d) { return y(d.value); })
            //        .style("stroke-width", 1)
            //        .style("stroke", function (d) { return d.color; });

            //var label_lines = svg.append("g").selectAll("text").data(lines_data);
            //label_lines.enter()
            //    .append("text")
            //        .attr("x", function (d) { return 50; })
            //        .attr("y", function (d) { return y(d.value) - 3; })
            //        .style({
            //            "font-family": "sans-serif",
            //            "font-size": "11px"
            //        })
            //        .text(function (d) { return d.label + ": " + d3.format(",.0f")(d.value); });
            //end

            // draw date phase
            //var schedule_data = [
            //    { date: "17-10-2014", label: "Begin Contract", color: "green" },
            //    { date: "21-03-2015", label: "End Contract", color: "red" }
            //];
            //var schedule_data_lines = svg.append("g").selectAll("line").data(schedule_data);
            //schedule_data_lines.enter()
            //    .append("line")
            //        .attr("x1", function (d) { return x(d.date); })
            //        .attr("y1", 0)
            //        .attr("x2", function (d) { return x(d.date); })
            //        .attr("y2", height)
            //        .style("stroke-width", 1)
            //        .style("stroke", function (d) { return d.color; });
            //var label_schedule_lines = svg.append("g").selectAll("text").data(schedule_data);
            //label_schedule_lines.enter()
            //    .append("text")
            //        .attr("x", function (d) { return x(d.date) + 3; })
            //        .attr("y", 10)
            //        .style({
            //            "font-family": "sans-serif",
            //            "font-size": "11px"
            //        })
            //        .text(function (d) { return d.label + ": " + d.date; });
            // end

            var legend = svg.selectAll(".legend")
                .data(phases.slice().reverse())
              .enter().append("g")
                .attr("class", "legend")
                .attr("transform", function (d, i) { return "translate(0," + i * 20 + ")"; });

            legend.append("rect")
                .attr("x", margin.left + margin.right + 10 - 10)
                .attr("width", 10)
                .attr("height", 10)
                .style("fill", colors)
                .style("stroke", "grey");

            legend.append("text")
                .attr("x", margin.left + margin.right + 10 - 12)
                .attr("y", 6)
                .attr("dy", ".35em")
                .style("text-anchor", "end")
                .text(function (d) { return d; });

            //draw line
            
            //end

        }

        var url_api = paramsView.urlGetCumulativeEffortByDate + "?projectCode=" + self.projectCode;

        d3.json(url_api, function (error, data) {
            if (error) {
                console.log(error);
            } else {
                console.log(data);
                renderCfd2(data);
            }
        });
        //console.log("afterRender");
    }
});