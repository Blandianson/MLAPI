function getInput(data) {
    
    var baseDataRowList = [];
    var baseData = [];

    var dataLines = data.split("\n");
    for (i = 0; i < dataLines.length; i++) {
        baseDataRowList.push(dataLines[i].split(","));
    }

    for (k = 0; k < baseDataRowList.length - 1; k++) {
        var isoDate = baseDataRowList[k][2];
        var date = isoDate.slice(4, 6) + "/" + isoDate.slice(6) + "/" + isoDate.slice(0, 4);
        var parseData = parseFloat(baseDataRowList[k][3])
        baseData.push([date, parseData]);
    }

    return baseData;
}

function initHighchart(baseData, stepSize, firstForecast, cleanArray) {

    ////Visualisation

    var myChart = Highcharts.chart('container', {
        chart: {
            borderWidth: 1,
            type: 'line',
            zoomType: 'x',
            panning: true,
            panKey: 'ctrl'
        },
        title: {
            text: 'Sales Quantity over Time'
        },
        xAxis: {
            type: 'datetime',
            labels: {
                step: 5
            },
            tickInterval: 20,
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
            name: 'Base Data',
            data: baseData,
            zoneAxis: 'x',
            zones: [{
                value: firstForecast
            }, {
                dashStyle: 'dot',
                color: 'orange'
            }],
        }]
    });

    myChart.addSeries({
        data: cleanArray,
        color: 'red'
    }, true);
}

function getCleaned(data) {

    var baseDataRowList = [];
    var baseData = [];

    var dataLines = data.split("\n");
    for (i = 0; i < dataLines.length; i++) {
        baseDataRowList.push(dataLines[i].split(","));
    }

    for (k = 1; k < baseDataRowList.length - 1; k++) {
        var isoDate = baseDataRowList[k][2];
        var date = isoDate.slice(5, 7) + "/" + isoDate.slice(8) + "/" + isoDate.slice(0, 4);
        var parseData = parseFloat(baseDataRowList[k][3])
        baseData.push([date, parseData]);
    }

    return baseData;
}

function collectData(inputArray, forecastArray) {
    return inputArray.concat(forecastArray)
}

$(document).ready(function () {
    var inputArray = getInput($("#inputData").val());
    var cleanArray = getCleaned($("#cleanedData").val());
    var forecastArray = getCleaned($("#forecastData").val());
    var allPoints = collectData(inputArray, forecastArray);
    console.log(allPoints)
    initHighchart(allPoints, allPoints.length/10, inputArray.length, cleanArray);
});