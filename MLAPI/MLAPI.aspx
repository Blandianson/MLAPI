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
<body>
    <form id="form1" runat="server">

        <div class="collapsible">

            <div class="collapsible-header">

                <div id="container" style="width:100%; height:400px; /*display:none;*/"></div>

            </div>


            <div class="collapsible-content">
                <div id="wrapperForm" class="container">

                    <div class="row">  <%--Server & Staging--%>
                        <div class="col-sm-6"><asp:Label runat="server" ID="sqlServerLabel" CssClass="fieldLabel">SQL Server <span class="required">*</span></asp:Label>
                        <asp:TextBox id="server" runat="server" AutoPostBack="False" Wrap="false" cssClass="inputTBoxes field-long" TextMode="MultiLine" Text="localhost"/></div>

                        <div class="col-sm-6"><asp:Label runat="server" ID="stagingDBLabel" CssClass="fieldLabel" value="No name specified">Staging Database Name</asp:Label>
                        <asp:TextBox id="staging" runat="server" AutoPostBack="False" Wrap="false" cssClass="inputTBoxes field-long" TextMode="MultiLine" Text="Optional"/></div>
                    </div>


                    <div class="row">  <%--File Name & Type--%>
                        <div class="col-sm-9"><asp:Label runat="server" ID="rScriptLabel" CssClass="fieldLabel">Script URI <span class="required">*</span></asp:Label>
                        <asp:TextBox id="rScript" runat="server" AutoPostBack="False" Wrap="false" cssClass="field-long" TextMode="MultiLine" Text="C:\\Users\\Nicole.jackson\\Documents\\Halo.Development\\R_Scripts_MLW\\testTimeSeries.r"/></div>
            
                        <div class="col-sm-3"><asp:Label runat="server" ID="fileTypeLabel" CssClass="fieldLabel">Script File Type <span class="required">*</span></asp:Label>
                        <div class="header"><asp:RadioButtonList ID="fileType" runat="server" onSelectedIndexChange="fileTypeChange" AutoPostBack="False" CssClass="fieldLabel">
                            <asp:ListItem Selected="true" Value="0"> R Script</asp:ListItem>
                            <asp:ListItem Value="1"> Python</asp:ListItem>
                        </asp:RadioButtonList></div></div>
                    </div>


                    <div class="row">  <%--Start & End Date--%>
                        <div class="col-sm-6"><asp:Label runat="server" ID="startDate" CssClass="fieldLabel">Time Series Data Start Date <span class="required">*</span></asp:Label>
                        <asp:TextBox ID="start" runat="server" cssClass="field-long" type="date" value="1921-01-01"></asp:TextBox></div>

                        <div class="col-sm-6"><asp:Label runat="server" ID="endDate" CssClass="fieldLabel">Time Series Data End Date <span class="required">*</span></asp:Label>
                        <asp:TextBox ID="end" runat="server" cssClass="field-long" type="date" value="1970-12-01"></asp:TextBox></div>
                    </div>


                    <div class="row"> <%--Column & Points--%>
                        <div class="col-sm-6"><asp:Label runat="server" ID="colNum" CssClass="fieldLabel">Column Number of Time Series Data <span class="required">*</span></asp:Label>
                        <asp:TextBox ID="column" runat="server" cssClass="field-long" type="number" value="2"></asp:TextBox></div>

                        <div class="col-sm-6"><asp:Label runat="server" ID="forecastRange" CssClass="fieldLabel">Number of Future Time Series Points <span class="required">*</span></asp:Label>
                        <asp:TextBox ID="forecast" runat="server" cssClass="field-long" type="number" value="20" ></asp:TextBox></div>
                    </div>

                
                    <div class="row"> <%--IO--%>
                        <div class="col-sm-3">
                            <asp:Label runat="server" ID="paramLabel" CssClass="fieldLabel">Time Series Data <span class="required">*</span></asp:Label>
                            <asp:TextBox id="parameters" runat="server" AutoPostBack="False" Wrap="false" cssClass="field-long" TextMode="MultiLine" Text="1,1" />
                        </div>
                
                        <div class="col-sm-9">
                            <div class="header" ><asp:Label runat="server" ID="outputResultLabel" CssClass="fieldLabel">Forecasted Results</asp:Label></div>
                            <asp:textbox id="outputText" runat="server" CssClass="textIObox field-long" TextMode="MultiLine"/>
                        </div>
                    </div>


                    <div class="row">
                        <div class="col-sm-12">
                            <div class="header"><asp:RadioButtonList ID="forecastOptions" runat="server" AutoPostBack="False" CssClass="fieldLabel">
                                <asp:ListItem Selected="true" Value="0"> Normal Forecast</asp:ListItem>
                                <asp:ListItem Selected="true" Value="0"> Enable ADAR Cleasing</asp:ListItem>
                                <asp:ListItem Value="1"> Enable ADAR Cleasing (with Holiday Consideration)</asp:ListItem>
                            </asp:RadioButtonList></div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-sm-12"><asp:Button ID="outputButton" runat="server" text="Return Forcast" AutoPostBack="True" onclick="Request_click"/></div>
                    </div>
                    <div class="row"> <%--Column & Points--%>
                        <div class="col-sm-12"><asp:Label runat="server" ID="debug" CssClass="fieldLabel">Debug <span class="required">*</span></asp:Label>
                        <asp:TextBox ID="debg" runat="server" cssClass="field-long" type="text"></asp:TextBox></div>
                    </div>

                 </div>
            </div>

        </div>
        <script type="text/javascript">

            $( function(){
	            // initialize collapsibles:
	            $( document ).trigger( "enhance" );
            });
        </script>

    </form>
</body>
</html>
