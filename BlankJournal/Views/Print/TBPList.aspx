<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="BlankJournal.Models" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>Перечень ТБП</title>
    <style>
        table, tr, td, p {
            font-family: 'Arial';
            font-size: 10pt;
        }

        h1, h2, h3, h4, h5, h6, hr {
            padding: 5;
            margin: 5;
        }


        h1 {
            font-family: 'Arial';
            font-size: 14pt;
        }

        h2 {
            font-family: 'Arial';
            font-size: 10pt;
        }

        table {
            border-collapse: collapse;
        }

        td, th {
            border-width: 1px;
            border-style: solid;
            border-color: #BBBBFF;
            padding-left: 10px;
            padding-right: 10px;
        }

        th {
            text-align: center;
        }

        td.center {
            text-align: center;
        }

        table td.bb table th.bb{
            border-bottom-width:medium;
        }

        table td.tb table th.tb{
            border-bottom-width:medium;
        }


        table td.right, table th.right {
            text-align: right;
        }

        table td.fill, table th.fill {
            background-color: Gray;
        }
    </style>
</head>
<body>
    <%Dictionary<string, List<TBPInfo>> TBPList = Model as Dictionary<string, List<TBPInfo>>;
      int index = 1;
      %>
    <div>       
        <table>
            <tr>
                <th>№пп</th>
                <th>№</th>
                <th>Имя</th>
            </tr>
            <%foreach (string folder in TBPList.Keys) { %>
            <tr>
                <th colspan="3"><%=folder %></th>
            </tr>
                <%foreach (TBPInfo tbp in TBPList[folder]) { %>
                <tr>
                    <td><%=index++ %></td>
                    <td><%=tbp.Number %></td>
                    <td><%=tbp.Name %></td>                   
                </tr>
            <%}} %>
        </table>
    </div>
</body>
</html>
