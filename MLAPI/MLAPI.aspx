﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MLAPI.aspx.cs" Inherits="HaloBI.Prism.Plugin.MLAPI" validateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	<link href="Styles/MLAPI.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="wrapperForm">
            <div class="header"><asp:Label runat="server" ID="apiUrlLabel" CssClass="fieldLabel">Api Url:</asp:Label></div>
            <asp:TextBox id="ApiUrl" runat="server" AutoPostBack="True" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine"/>
            <div class="header"><asp:Label runat="server" ID="dataLabel" CssClass="fieldLabel">Input Data:</asp:Label></div>
            <asp:TextBox id="data" runat="server" Wrap="false" cssClass="inputTBoxes" TextMode="MultiLine"/>
            <asp:Button ID="call" runat="server" text="Request Forcast" AutoPostBack="True" onclick="submitApiData_Click"/>
            <div id="resultBox">
                <div class="header"><asp:Label runat="server" ID="resultLabel" CssClass="fieldLabel">Results:</asp:Label></div>
                <asp:Button ID="copyAll" runat="server" text="Copy All" AutoPostBack="True" onclick="rabbitMessaging_click"/>
                <asp:textbox id="postData" runat="server" CssClass="textIObox" TextMode="MultiLine"/>
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
