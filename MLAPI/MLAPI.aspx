<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MLAPI.aspx.cs" Inherits="HaloBI.Prism.Plugin.MLAPI" validateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	<link href="Styles/MLAPI.css" rel="stylesheet" />
    <script type="text/javascript" src="https://code.jquery.com/jquery-3.3.1.min.js"></script>
    <script type="text/javascript" src="http://code.highcharts.com/highcharts.js"></script>
    <script type="text/javascript" src="https://code.highcharts.com/modules/exporting.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="wrapperForm">

            <div class="header"><asp:Label runat="server" ID="sessionLabel" CssClass="fieldLabel">Session ID</asp:Label></div>
            <asp:TextBox id="sessionID" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="38CE52DA-BE16-45C5-A0C8-D90EE9A07ED6"/>
            <div class="header"><asp:Label runat="server" ID="executionLabel" CssClass="fieldLabel">Execution ID</asp:Label></div>
            <asp:TextBox id="executionID" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="100"/>
            <div class="header"><asp:Label runat="server" ID="sqlServerLabel" CssClass="fieldLabel">SQL Server *</asp:Label></div>
            <asp:TextBox id="server" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="localhost"/>
            <div class="header"><asp:Label runat="server" ID="stagingDBLabel" CssClass="fieldLabel">Staging Database Name</asp:Label></div>
            <asp:TextBox id="staging" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="Optional"/>
            <div class="header"><asp:Label runat="server" ID="fileTypeLabel" CssClass="fieldLabel">Script File Type *</asp:Label></div>
            <div class="header"><asp:RadioButtonList ID="fileType" runat="server" onSelectedIndexChange="fileTypeChange" AutoPostBack="true" CssClass="fieldLabel">
                <asp:ListItem Selected="true" Value="0"> R Script</asp:ListItem>
                <asp:ListItem Value="1"> Python</asp:ListItem>
            </asp:RadioButtonList></div>
            <div class="header"><asp:Label runat="server" ID="rScriptLabel" CssClass="fieldLabel">Script URI *</asp:Label></div>
            <asp:TextBox id="rScript" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="C:\\Users\\Nicole.jackson\\Documents\\Halo.Development\\R_Scripts_MLW\\testTimeSeries.r"/>
            <div class="header"><asp:Label runat="server" ID="paramLabel" CssClass="fieldLabel">Parameters * (Comma Separated. Rows split with a newline.)</asp:Label></div>
            <asp:TextBox id="parameters" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="1,1"/>
            <div class="header"><asp:Label runat="server" ID="startDate" CssClass="fieldLabel">Time Series Data Start Date *</asp:Label></div>
            <asp:TextBox ID="start" runat="server" cssClass="inputTBoxes" type="date" value="1921-01-01"></asp:TextBox>
            <div class="header"><asp:Label runat="server" ID="endDate" CssClass="fieldLabel">Time Series Data End Date *</asp:Label></div>
            <asp:TextBox ID="end" runat="server" cssClass="inputTBoxes" type="date" value="1970-12-01"></asp:TextBox>
            <div class="header"><asp:Label runat="server" ID="colNum" CssClass="fieldLabel">Column Number of Time Series Data *</asp:Label></div>
            <asp:TextBox ID="column" runat="server" cssClass="inputTBoxes" type="number" value="2"></asp:TextBox>
            <div class="header"><asp:Label runat="server" ID="forecastRange" CssClass="fieldLabel">Number of Future Time Series Points *</asp:Label></div>
            <asp:TextBox ID="forecast" runat="server" cssClass="inputTBoxes" type="number" value="20"></asp:TextBox>

            <asp:Button ID="call" runat="server" text="Request Forcast" AutoPostBack="True" onclick="RabbitMessaging_click"/>
            <div class="resultBox">
                <div class="header"><asp:Label runat="server" ID="resultLabel" CssClass="fieldLabel">Input Results:</asp:Label></div>
                <asp:textbox id="postData" runat="server" CssClass="textIObox" TextMode="MultiLine"/>
            </div>

            <asp:Button ID="outputButton" runat="server" text="Return Forcast" AutoPostBack="True" onclick="Request_click"/>
            <div class="resultBox">
                <div class="header"><asp:Label runat="server" ID="outputResultLabel" CssClass="fieldLabel">Output Results:</asp:Label></div>
                <asp:textbox id="outputText" runat="server" CssClass="textIObox" TextMode="MultiLine"/>
            </div>

            <div id="container" style="width:100%; height:400px;"></div>

            <script type="text/javascript">
                //console.log($("#outputText").val());
                let resultStr = $("#outputText").val()
                let dataStr = resultStr.slice(resultStr.indexOf("Jan"));
                let dataList = dataStr.split("\n");
                //dataList = dataList.map(x => x.split(new RegExp("\\s+")));
                let dataRowList = dataList.map(x => x.split("[a-z][A-Z]"));
                console.log(dataRowList);
                $(function () { 
                    var myChart = Highcharts.chart('container', {
                        chart: {
                            type: 'line'
                        },
                        title: {
                            text: 'Lake Erie Water Levels'
                        },
                        xAxis: {
                            categories: ["1921 Jan", "1921 Feb", "1921 Mar", "1921 Apr", "1921 May", "1921 Jun", "1921 Jul", "1921 Aug", "1921 Sept", "1921 Oct", "1921 Nov", "1921 Dec", "1922 Jan", "1922 Feb", "1922 Mar", "1922 Apr", "1922 May", "1922 Jun", "1922 Jul", "1922 Aug", "1922 Sept", "1922 Oct", "1922 Nov", "1922 Dec", "1923 Jan"]
                        },
                        yAxis: {
                            title: {
                                text: 'Water Level (m)'
                            }
                        },
                        series: [{
                            name: 'Time (Monthly)',
                            data: [ 14.763, 14.649, 15.085, 16.376, 16.926, 16.774, 16.490, 15.769, 15.180, 14.383, 14.478, 14.364, 13.928, 13.283, 13.700, 15.465, 16.243, 16.490, 16.243, 15.787, 15.446, 14.649, 13.776, 13.188, 13.283]
                        }]
                    });
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
