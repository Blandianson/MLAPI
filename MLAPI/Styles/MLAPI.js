$(document).ready(function () {

    //Highcharts Config

    let rawBaseData = $("#parameters").val().trim();
    let baseDataList = rawBaseData.split("\n");

    let baseDataRowList = [];

    for (i = 0; i < baseDataList.length; i++) {
        baseDataRowList.push(baseDataList[i].split(","));
    }

    let baseDate = [];
    let baseData = [];

    for (k = 0; k < baseDataRowList.length; k++) {
        let ISODate = baseDataRowList[k][0];
        ISODate = ISODate.replace(/"/g, '');
        ISODate = ISODate.split(/\-/);
        ISODate = new Date(parseInt(ISODate[0]), parseInt(ISODate[1]) - 1);
        baseDate.push(ISODate.getFullYear());
        baseData.push(parseFloat(baseDataRowList[k][1]));
    }

    //Forecasted Data

    let resultStr = $("#outputText").val();
    let dataStr = resultStr.slice(resultStr.indexOf("Jan"));
    dataStr = dataStr.trim();
    let dataList = dataStr.split("\n");

    let dataRowList = [];

    for (i = 0; i < dataList.length; i++) {
        dataRowList.push(dataList[i].trim().split(/\s+/));

    }

    let dateList = [];
    let forecastDataList = [];
    let firstDataPt = baseDate.length;

    for (i = 0; i < dataRowList.length; i++) {
        baseDate.push(dataRowList[i][1]);
        baseData.push(parseFloat(dataRowList[i][2]));
    }

    let stepSize = baseDate.length / 100;

    //Visualisation


    var myChart = Highcharts.chart('container', {
        chart: {
            borderWidth: 1,
            type: 'line',
            zoomType: 'x',
            panning: true,
            panKey: 'ctrl'
        },
        title: {
            text: 'Lake Erie Water Levels'
        },
        xAxis: {
            categories: baseDate,
            labels: {
                step: 5
            },
            tickInterval: 20
        },
        yAxis: {
            title: {
                text: 'Water Level (m)'
            }
        },
        series: [{
            name: 'Forecast',
            data: baseData,
            zoneAxis: 'x',
            zones: [{
                value: firstDataPt
            }, {
                dashStyle: 'dot',
                color: 'orange'
            }],
        }]
    });
});