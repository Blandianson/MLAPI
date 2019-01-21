<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MLAPI.aspx.cs" Inherits="HaloBI.Prism.Plugin.MLAPI" validateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	<link href="Styles/MLAPI.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"/>
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.3.1.min.js"></script>
    <script type="text/javascript" src="http://code.highcharts.com/highcharts.js"></script>
    <script type="text/javascript" src="https://code.highcharts.com/modules/exporting.js"></script>
    <script type="text/javascript" src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="wrapperForm" class="container">

            <%--<div class="header"><asp:Label runat="server" ID="sessionLabel" CssClass="fieldLabel">Session ID</asp:Label></div>
            <asp:TextBox id="sessionID" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="38CE52DA-BE16-45C5-A0C8-D90EE9A07ED6"/>

            <div class="header"><asp:Label runat="server" ID="executionLabel" CssClass="fieldLabel">Execution ID</asp:Label></div>
            <asp:TextBox id="executionID" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="100"/>--%>

            <div class="row">  <%--Server & Staging--%>
                <div class="col-sm-6"><asp:Label runat="server" ID="sqlServerLabel" CssClass="fieldLabel">SQL Server *</asp:Label>
                <br />
                <asp:TextBox id="server" runat="server" AutoPostBack="False" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="localhost"/></div>

                <div class="col-sm-6"><asp:Label runat="server" ID="stagingDBLabel" CssClass="fieldLabel" value="No name specified">Staging Database Name</asp:Label>
                    <br />
                <asp:TextBox id="staging" runat="server" AutoPostBack="False" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="Optional"/></div>
            </div>


            <div class="row">  <%--File Name & Type--%>
                <div class="col-sm-8"><asp:Label runat="server" ID="rScriptLabel" CssClass="fieldLabel">Script URI *</asp:Label>
                    <br />
                <asp:TextBox id="rScript" runat="server" AutoPostBack="False" Wrap="false" cssClass="inputTBoxes field-long" TextMode="MultiLine" Text="C:\\Users\\Nicole.jackson\\Documents\\Halo.Development\\R_Scripts_MLW\\testTimeSeries.r"/></div>
            
                <div class="col-sm-4"><asp:Label runat="server" ID="fileTypeLabel" CssClass="fieldLabel">Script File Type *</asp:Label>
                    <br />
                <div class="header"><asp:RadioButtonList ID="fileType" runat="server" onSelectedIndexChange="fileTypeChange" AutoPostBack="False" CssClass="fieldLabel">
                    <asp:ListItem Selected="true" Value="0"> R Script</asp:ListItem>
                    <asp:ListItem Value="1"> Python</asp:ListItem>
                </asp:RadioButtonList></div></div>
            </div>


            <div class="row">  <%--Start & End Date--%>
                <div class="col-sm-6"><asp:Label runat="server" ID="startDate" CssClass="fieldLabel">Time Series Data Start Date *</asp:Label>
                    <br />
                <asp:TextBox ID="start" runat="server" cssClass="inputTBoxes" type="date" value="1921-01-01"></asp:TextBox></div>

                <div class="col-sm-6"><asp:Label runat="server" ID="endDate" CssClass="fieldLabel">Time Series Data End Date *</asp:Label>
                    <br />
                <asp:TextBox ID="end" runat="server" cssClass="inputTBoxes" type="date" value="1970-12-01"></asp:TextBox></div>
            </div>


            <div class="row"> <%--Column & Points--%>
                <div class="col-sm-6"><asp:Label runat="server" ID="colNum" CssClass="fieldLabel">Column Number of Time Series Data *</asp:Label>
                <br />
                <asp:TextBox ID="column" runat="server" cssClass="inputTBoxes" type="number" value="2"></asp:TextBox></div>

                <div class="col-sm-6"><asp:Label runat="server" ID="forecastRange" CssClass="fieldLabel">Number of Future Time Series Points *</asp:Label>
                <br />
                <asp:TextBox ID="forecast" runat="server" cssClass="inputTBoxes" type="number" value="20" ></asp:TextBox></div>
            </div>

                
            <div class="row"> <%--IO--%>
                <div class="col-sm-3">
                    <asp:Label runat="server" ID="paramLabel" CssClass="fieldLabel">Time Series Data *</asp:Label>
                    <br />
                    <asp:TextBox id="parameters" runat="server" AutoPostBack="False" Wrap="false" cssClass="inputTBoxes field-long" TextMode="MultiLine" Text="1,1" />
                </div>
                
                <div class="col-sm-9">
                    <div class="header" ><asp:Label runat="server" ID="outputResultLabel" CssClass="fieldLabel">Forecasted Results:</asp:Label></div>
                    <br />
                    <asp:textbox id="outputText" runat="server" CssClass="textIObox field-long" TextMode="MultiLine"/>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12"><asp:Button ID="outputButton" runat="server" text="Return Forcast" AutoPostBack="True" onclick="Request_click"/></div>
            </div>

            <div id="container" style="width:100%; height:400px;"></div>

            <script type="text/javascript">

                let rawBaseData = $("#parameters").val().trim();
                let baseDataList = rawBaseData.split("\n");

                let baseDataRowList = [];

                for (i = 0; i < baseDataList.length; i++) {
                    baseDataRowList.push(baseDataList[i].split(","));
                }

                let baseDate = [];
                let baseData = [];

                for (k = 0; k < baseDataRowList.length; k++) {
                    //let ISODate = (baseDataRowList[k][0].replace(/\"/, "").split(/\-/)).forEach(x => parseInt(x));
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

                for (i = 0; i < dataList.length; i++){
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
                        type: 'line'
                    },
                    title: {
                        text: 'Lake Erie Water Levels'
                    },
                    xAxis: {
                        categories: baseDate, //["1921 Jan", "1921 Feb", "1921 Mar", "1921 Apr", "1921 May", "1921 Jun", "1921 Jul", "1921 Aug", "1921 Sept", "1921 Oct", "1921 Nov", "1921 Dec", "1922 Jan", "1922 Feb", "1922 Mar", "1922 Apr", "1922 May", "1922 Jun", "1922 Jul", "1922 Aug", "1922 Sept", "1922 Oct", "1922 Nov", "1922 Dec", "1923 Jan"],
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
                        data: baseData,  //[ 14.763, 14.649, 15.085, 16.376, 16.926, 16.774, 16.490, 15.769, 15.180, 14.383, 14.478, 14.364, 13.928, 13.283, 13.700, 15.465, 16.243, 16.490, 16.243, 15.787, 15.446, 14.649, 13.776, 13.188, 13.283]
                        zoneAxis: 'x',
                        zones: [{
                            value: firstDataPt
                        }, {
                            dashStyle: 'dot',
                            color: 'orange'
                        }],
                    }]
                });

            </script>
            </div>

    <div id="hideAdmin">
		<h3>
			<asp:Label ID="uiSelectedMembers" 
				runat="server"
				cssClass="ui-plugin-template-prismSelection">
			</asp:Label>
		</h3>
		
		<div>
			<asp:DropDownList ID="uiMembersList" 
				runat="server" 
				OnSelectedIndexChanged="UIMembersList_SelectedIndexChanged"
				AutoPostBack="true">
			</asp:DropDownList>
<%--			<asp:Button ID="uiUpdatePrism" 
				runat="server" 
				OnClick="uiUpdatePrism_Click"
				Text="Update Prism" 
			/>--%>
		</div>
<%--		<div>
			<h3><asp:Label runat="server">Shared Contxt Object</asp:Label></h3>
			<asp:TextBox 
				ID="uiContext" 
				TextMode="MultiLine"
				Height = "400px"
				Width = "100%"
				runat="server"
				Visible="false"
				CssClass="ui-plugin-template-context">
			</asp:TextBox>
		</div>--%>

    </div>
    </form>
</body>
</html>
