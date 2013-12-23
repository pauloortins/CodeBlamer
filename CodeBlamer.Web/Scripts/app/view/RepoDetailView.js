var RepoDetailView = Class.extend({
    init: function() {
        google.load("visualization", "1", { packages: ["corechart"] });
        this._createBinds();
        this._initElements();
    },
    
    _createBinds: function() {
        this._tree = $("#tree");
    },
    
    _initElements: function () {
        var that = this;

        that._tree.on('acitree', function (event, api, item, eventName, options) {
            if (eventName == 'selected') {
                var itemData = api.itemData(item);
                var node = itemData['node'];

                $.get(repoDetailUrl.urlNodeMetrics, { repositoryUrl: "John", node: node })
                  .done(function (data) {
                      var chartOptions = {
                          title: 'Company Performance',
                          curveType: 'function',
                          legend: { position: 'bottom' },
                          pointSize: 3
                      };

                      var chart = new google.visualization.LineChart(document.getElementById('chart_div'));
                      chart.draw(that._convertToChart(data), chartOptions);
                  });
            }
        });
        
        that._tree.aciTree({
            ajax: {
                url: repoDetailUrl.urlProjectTree
            },
            selectable: true
        });
    },
    
    _convertToChart: function convertToChart(metricsData) {
        var data = new google.visualization.DataTable();
        data.addColumn('date', 'Commits');
        data.addColumn('number', 'MaintainabilityIndex');
        data.addColumn('number', 'CyclomaticComplexity');
        data.addColumn('number', 'ClassCoupling');
        data.addColumn('number', 'DepthOfInheritance');
        data.addColumn('number', 'LinesOfCode');
        
        data.addRows(metricsData.length);

        for (var i = 0; i < metricsData.length; i++) {
            data.setCell(i, 0, this._parseJsonDate(metricsData[i].Date));
            data.setCell(i, 1, metricsData[i].Metrics["MaintainabilityIndex"]);
            data.setCell(i, 2, metricsData[i].Metrics["CyclomaticComplexity"]);
            data.setCell(i, 3, metricsData[i].Metrics["ClassCoupling"]);
            data.setCell(i, 4, metricsData[i].Metrics["DepthOfInheritance"]);
            data.setCell(i, 5, metricsData[i].Metrics["LinesOfCode"]);
        }

        return data;
    },
    
    _parseJsonDate: function (jsonDateString) {
        return new Date(parseInt(jsonDateString.replace('/Date(', '')));
    }
});