<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Demo.aspx.cs" Inherits="composite_scripts.Demo" %>
<%@ Register Assembly="composite-scripts" Namespace="CompositeScripts" TagPrefix="cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Composite Scripts Demo</title>
</head>
<body>
    <form id="form1" runat="server">

    <cs:CompositeScriptManager ID="csmDemo" runat="server" >
        <Scripts>
            <asp:ScriptReference path="Scripts/a.js" />
            <asp:ScriptReference path="Scripts/b.js" />
            <asp:ScriptReference path="Scripts/c.js" />
        </Scripts>
        <StyleSheets>
            <asp:ScriptReference path="Styles/a.css" />
            <asp:ScriptReference path="Styles/b.css" />
            <asp:ScriptReference path="Styles/c.css" />
        </StyleSheets>
    </cs:CompositeScriptManager>

    <div>
    </div>
    </form>
</body>
</html>
