<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>SyncDB</title>
</head>
<body>
    <div>
      <%List<String> answer = Model as List<String>;  %>
        <% foreach (string str in answer){ %>
        <h3><%=str %></h3>
        <%} %>
    </div>
</body>
</html>
