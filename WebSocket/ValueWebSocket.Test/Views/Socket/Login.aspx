﻿<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login</title>
</head>
<body>
    <div>
        <%using (Html.BeginForm("Login", "Socket"))
          {%>
        <%=Html.TextBox("userName") %>
        <input type="submit" value="登陆" />
        <%} %>
    </div>
</body>
</html>
