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
    <script type="text/javascript" src="Styles/MLAPI.js"></script>
</head>
<body runat="server">
    <form id="form1" runat="server">
        <div class="collapsible-header">
            <div id="container" style="width:100%; height:400px;"></div>
        </div>

        <div id="wrapperForm" class="container">

            <div class="row"> <%--Column & Points--%>
                <div class="col-sm-6"><asp:Label runat="server" ID="colNum" CssClass="fieldLabel">Column Number of Time Series Data <span class="required">*</span></asp:Label>
                <asp:TextBox ID="column" runat="server" cssClass="field-long" type="number" value="2"></asp:TextBox></div>

                <div class="col-sm-6"><asp:Label runat="server" ID="forecastRange" CssClass="fieldLabel">Number of Future Time Series Points <span class="required">*</span></asp:Label>
                <asp:TextBox ID="forecast" runat="server" cssClass="field-long" type="number" value="20" ></asp:TextBox></div>
            </div>

                
            <div class="row"> <%--IO--%>                
                <div class="col-sm-12">
                    <div class="header" ><asp:Label runat="server" ID="outputResultLabel" CssClass="fieldLabel">Forecasted Results</asp:Label></div>
                    <asp:textbox id="outputText" runat="server" CssClass="textIObox field-long" TextMode="MultiLine"/>
                </div>
            </div>


            <div class="row"> <%--Forecast Type--%>   
                <div class="col-sm-12">
                    <div class="header" runat="server">
                        <input type="radio" id="normal" name="forecastRadio" value="0"/>
                        <label for="normal"">Normal Forecast</label>
                        <br />
                        <input type="radio" id="ADAR" name="forecastRadio" value="1" checked="checked"/>
                        <label for="normal"">Enable ADAR Cleansing</label>
                        <%-- <asp:ListItem Selected="true" Value="1"> Enable ADAR Cleansing</asp:ListItem>--%>
                        <br />
                        <input type="radio" id="ADARhol" name="forecastRadio" value="2" />
                        <label for="normal"">Enable ADAR Cleansing (with Holiday Consideration)</label>
                        <%--<asp:ListItem Value="2"> Enable ADAR Cleansing (with Holiday Consideration)</asp:ListItem>--%>
                    </div>
                </div>
            </div>


            <div class="row"> <%--Go Button--%>   
                <div class="col-sm-12"><asp:Button ID="outputButton" runat="server" text="Return Forcast" AutoPostBack="True" onclick="Request_click"/></div>
            </div>                


            <div class="row" style="display:none"> <%--Input Test--%>
                <div class="col-sm-12"><asp:Label runat="server" ID="inpt" CssClass="fieldLabel">Input</asp:Label>
                <asp:TextBox ID="inputData" runat="server" cssClass="field-long" type="text" TextMode="MultiLine"></asp:TextBox></div>
            </div>

            <div class="row" style="display:none"> <%--Cleaned Test--%>
                <div class="col-sm-12"><asp:Label runat="server" ID="clnd" CssClass="fieldLabel">Cleaned</asp:Label>
                <asp:TextBox ID="cleanedData" runat="server" cssClass="field-long" type="text" TextMode="MultiLine"></asp:TextBox></div>
            </div>

            <div class="row" style="display:none"> <%--Forecast Test--%>
                <div class="col-sm-12"><asp:Label runat="server" ID="frcst" CssClass="fieldLabel">Forecast</asp:Label>
                <asp:TextBox ID="forecastData" runat="server" cssClass="field-long" type="text" TextMode="MultiLine"></asp:TextBox></div>
            </div>
        </div>
    </form>
</body>
</html>
