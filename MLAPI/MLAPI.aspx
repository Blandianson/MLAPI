<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MLAPI.aspx.cs" Inherits="HaloBI.Prism.Plugin.MLAPI" validateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	<link href="Styles/MLAPI.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="wrapperForm">

            <div class="header"><asp:Label runat="server" ID="sessionLabel" CssClass="fieldLabel">Session ID:</asp:Label></div>
            <asp:TextBox id="sessionID" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="38CE52DA-BE16-45C5-A0C8-D90EE9A07ED6"/>
            <div class="header"><asp:Label runat="server" ID="executionLabel" CssClass="fieldLabel">Execution ID:</asp:Label></div>
            <asp:TextBox id="executionID" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="100"/>
            <div class="header"><asp:Label runat="server" ID="sqlServerLabel" CssClass="fieldLabel">SQL Server:</asp:Label></div>
            <asp:TextBox id="server" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="localhost"/>
            <div class="header"><asp:Label runat="server" ID="stagingDBLabel" CssClass="fieldLabel">Staging Database Name:</asp:Label></div>
            <asp:TextBox id="staging" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="xxx"/>
            <div class="header"><asp:Label runat="server" ID="fileTypeLabel" CssClass="fieldLabel">Script File Type:</asp:Label></div>
            <div class="header"><asp:RadioButtonList ID="fileType" runat="server" onSelectedIndexChange="fileTypeChange" AutoPostBack="true" CssClass="fieldLabel">
                <asp:ListItem Selected="true" Value="0"> R Script</asp:ListItem>
                <asp:ListItem Value="1"> Python</asp:ListItem>
            </asp:RadioButtonList></div>
            <div class="header"><asp:Label runat="server" ID="rScriptLabel" CssClass="fieldLabel">Script URI:</asp:Label></div>
            <asp:TextBox id="rScript" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="C:\\Users\\Nicole.jackson\\Documents\\Halo.Development\\R_Scripts_MLW\\test_add.r"/>
            <div class="header"><asp:Label runat="server" ID="paramLabel" CssClass="fieldLabel">Parameters (Comma Separated, no spaces):</asp:Label></div>
            <asp:TextBox id="parameters" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine" Text="1,1"/>

            <asp:Button ID="call" runat="server" text="Request Forcast" AutoPostBack="True" onclick="rabbitMessaging_click"/>
            <div class="resultBox">
                <div class="header"><asp:Label runat="server" ID="resultLabel" CssClass="fieldLabel">Input Results:</asp:Label></div>
                <asp:textbox id="postData" runat="server" CssClass="textIObox" TextMode="MultiLine"/>
            </div>

            <asp:Button ID="outputButton" runat="server" text="Return Forcast" AutoPostBack="True" onclick="request_click"/>
            <div class="resultBox">
                <div class="header"><asp:Label runat="server" ID="outputResultLabel" CssClass="fieldLabel">Output Results:</asp:Label></div>
                <asp:textbox id="outputText" runat="server" CssClass="textIObox" TextMode="MultiLine"/>
            </div>
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
				OnSelectedIndexChanged="uiMembersList_SelectedIndexChanged"
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
