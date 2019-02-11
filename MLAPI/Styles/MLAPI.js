var mychart;
var demo;
var real;

//initalise the graph with base data and forecast
function initHighchart(baseData) {
    
    myChart = Highcharts.chart('container', {
        chart: {
            borderWidth: 1,
            type: 'spline',
            zoomType: 'x',
            panning: true,
            panKey: 'ctrl'
        },
        title: {
            text: 'Sales Quantity over Time'
        },
        xAxis: {
            type: 'datetime',
            dateTimeLabelFormats: {
                month: '%e. %b',
                year: '%b'
            },
            title: {
                text: 'Date'
            }
        },
        yAxis: {
            title: {
                text: 'Sales'
            }
        },
        series: [{
            name: 'Base History',
            data: baseData,
            zoneAxis: 'x',
            color: '#000000',  
            labels: {
                format: '{value: %Y-%m-%d}'
            }
        }],
        tooltip: {enabled: false}
    });

}

//Add ADAR data to graph
function addSeries(newData, title, color, dash){

    myChart.addSeries({
        data: newData,
        color: color,
        name: title,
        dashStyle: dash
    }, true);

}

//Formats the data to how Highcharts accepts data.
function formatTimeSeries(data, startYr, endYr, startMnth, endMnth, startDay) {
    
    var baseDataRowList = [];
    var baseData = [];

    var dataLines = data.split("\n");
    for (i = 0; i < dataLines.length; i++) {
        baseDataRowList.push(dataLines[i].split(","));
    }

    for (k = 0; k < baseDataRowList.length - 1; k++) {
        var isoDate = baseDataRowList[k][2];
        var date = Date.UTC(parseFloat(isoDate.slice(startYr, endYr)), parseFloat(isoDate.slice(startMnth, endMnth)) - 1, parseFloat(isoDate.slice(startDay)));
        var parseData = parseFloat(baseDataRowList[k][3])
        baseData.push([date, parseData]);
    }

    return baseData;
}

//Main function
$(document).ready(function () {
    var inputArray;
    var cleanArray;
    var forecastArray;
    var actualForecast;

    //plot Base History
    inputArray = formatTimeSeries($("#inputData").val(), 0, 4, 4, 6, 6);
    initHighchart(inputArray);

    //plot ADAR cleaned History
    if ($("#cleanedData").val() != "") {
        cleanArray = formatTimeSeries($("#cleanedData").val(), 0, 4, 5, 7, 8);
        addSeries(cleanArray, "Cleaned History", "#000000", 'longdash');
    }
    if ($("#cleanForecast").val() != "") {
        forecastArray = formatTimeSeries($("#cleanForecast").val(), 0, 4, 5, 7, 8);
        addSeries(forecastArray, "Cleaned Forecast", "#307af2", 'none');
    }
    if ($("#actualForecast").val() != "") {
        actualForecast = formatTimeSeries($("#actualForecast").val(), 0, 4, 5, 7, 8);
        addSeries(actualForecast, "Base Forecast", "#c92e2e", 'none');
    }
    if ($("#holidayForecast").val() != "") {
        actualForecast = formatTimeSeries($("#holidayForecast").val(), 0, 4, 5, 7, 8);
        addSeries(actualForecast, "Holiday Oversampling Forecast", "#c92e2e", 'none');
    }

});