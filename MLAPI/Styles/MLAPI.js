var mychart;

//initalise the graph with base data and forecast
function initHighchart(baseData, firstForecast) {
    
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
            name: 'Base Data',
            data: baseData,
            zoneAxis: 'x',
            color: '#b3b3ff',
            zones: [{
                value: firstForecast
            }, {
                color: 'orange'
            }],
        }]
    });
}

//Add ADAR data to graph
function showCleanedData(cleanArray){

    myChart.addSeries({
        data: cleanArray,
        color: '#4d4dff',
        name: 'ADAR'
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
        var date = new Date(parseFloat(isoDate.slice(startYr, endYr)), parseFloat(isoDate.slice(startMnth, endMnth)) - 1, parseFloat(isoDate.slice(startDay)));
        var parseData = parseFloat(baseDataRowList[k][3])
        baseData.push([date, parseData]);
    }

    return baseData;
}

//Aggregates the seperate data sources to one Array
function collectData(inputArray, forecastArray) {
    return inputArray.concat(forecastArray)
}

//Show Cleaned Data and/or Holiday Consideration
function additionalOptions(cleanArray) {
    var radio = document.getElementsByName("forecastRadio");
    
    for (var i = 0; i < radio.length; i++) {

        if (radio[i].checked && radio[i].value == "1") {

            showCleanedData(cleanArray);

        }
    }
}

//Main function
$(document).ready(function () {
    var inputArray = formatTimeSeries($("#inputData").val(), 0, 4, 4, 6, 6);
    var cleanArray = formatTimeSeries($("#cleanedData").val(), 0, 4, 5, 7, 8);
    var forecastArray = formatTimeSeries($("#forecastData").val(), 0, 4, 5, 7, 8);
    var allPoints = collectData(inputArray, forecastArray);
    initHighchart(allPoints, inputArray.length);

    additionalOptions(cleanArray);
});