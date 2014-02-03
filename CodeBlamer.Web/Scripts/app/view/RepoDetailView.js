var RepoDetailView = Class.extend({
    init: function () {                
        this._createBinds();
        this._initElements();
    },
    
    _createBinds: function() {
        this._tree = $("#tree");
        this.lblRepositoryUrl = $("#lblRepositoryUrl");
    },
    
    _initElements: function () {
        var that = this;

        that._tree.on('acitree', function (event, api, item, eventName, options) {
            if (eventName == 'selected') {
                var itemData = api.itemData(item);
                var node = itemData['node'];

                $.get(repoDetailUrl.urlNodeMetrics, { repositoryUrl: that.lblRepositoryUrl.html(), node: node })
                  .done(function (data) {
                      var chartOptions = {
                          title: '',
                          curveType: 'function',
                          legend: { position: 'bottom' },
                          pointSize: 3
                      };

                      var powerMetricChart = new google.visualization.LineChart(document.getElementById('powerMetricsChart'));
                      powerMetricChart.draw(that._convertToPowerMetricChart(data), chartOptions);
                      
                      var fxCopChart = new google.visualization.LineChart(document.getElementById('fxCopMetricsChart'));
                      fxCopChart.draw(that._convertToFxCopChart(data), chartOptions);
                      
                      var styleCopChart = new google.visualization.LineChart(document.getElementById('styleCopMetricsChart'));
                      styleCopChart.draw(that._convertToStyleCopChart(data), chartOptions);
                  });
            }
        });
        
        that._tree.aciTree({
            ajax: {
                url: repoDetailUrl.urlProjectTree,
                data: { repositoryUrl: that.lblRepositoryUrl.html()}
            },
            selectable: true
        });
    },    
    
    _convertToPowerMetricChart: function convertToChart(metricsData) {
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
            data.setCell(i, 1, metricsData[i].PowerMetrics["MaintainabilityIndex"]);
            data.setCell(i, 2, metricsData[i].PowerMetrics["CyclomaticComplexity"]);
            data.setCell(i, 3, metricsData[i].PowerMetrics["ClassCoupling"]);
            data.setCell(i, 4, metricsData[i].PowerMetrics["DepthOfInheritance"]);
            data.setCell(i, 5, metricsData[i].PowerMetrics["LinesOfCode"]);
        }

        return data;
    },
    
    _convertToFxCopChart: function convertToChart(metricsData) {
        var data = new google.visualization.DataTable();
        data.addColumn('date', 'Commits');
        data.addColumn('number', 'NumberOfIssues');
        
        data.addRows(metricsData.length);

        for (var i = 0; i < metricsData.length; i++) {
            data.setCell(i, 0, this._parseJsonDate(metricsData[i].Date));
            data.setCell(i, 1, metricsData[i].FxCopMetrics["NumberOfIssues"]);
        }

        return data;
    },
    
    _convertToStyleCopChart: function convertToChart(metricsData) {
        var data = new google.visualization.DataTable();
        data.addColumn('date', 'Commits');
        data.addColumn('number', 'NumberOfIssues');

        data.addRows(metricsData.length);

        for (var i = 0; i < metricsData.length; i++) {
            data.setCell(i, 0, this._parseJsonDate(metricsData[i].Date));
            data.setCell(i, 1, metricsData[i].StyleCopMetrics["NumberOfIssues"]);
        }

        return data;
    },
    
    _parseJsonDate: function (jsonDateString) {
        return new Date(parseInt(jsonDateString.replace('/Date(', '')));
    }
});