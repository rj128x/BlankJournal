<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="BlankJournal.Models" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>PrintComments</title>
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
    <%BlankJournal.CommentAnswer answer = Model as BlankJournal.CommentAnswer;  %>
    <%List<TBPComment> data = answer.Data;
      data.Reverse();
      int index = 0;     
    %>
    <div>
        <h1>Замечания к ТБП с <u><%=answer.dateStart.Value.ToString("dd.MM.yyyy")%></u> по <u><%=answer.dateEnd.Value.ToString("dd.MM.yyyy")%></u></h1>
        <table>
            <tr>
                <th style="width: 30px;">№пп</th>
                <th style="width: 100px;">Автор</th>
                <th>Замечание</th>
                <th style="width: 100px;">Закрыл</th>
                <th style="width: 100px;">Комментарий</th>
            </tr>
            <%foreach (TBPComment  rec in data) { %>
            <tr>
                <td><%=++index %></td>
                <td><%=rec.Author %><br /><%=rec.DateCreate.ToString("dd.MM.yyyy") %></td>
                <td><%=rec.CommentText %></td>
                <td><%=rec.Finished?rec.Performer+"<br/>"+rec.DatePerform.ToString("dd.MM.yyyy"):"-" %></td>
                <td><%=rec.Finished?rec.CommentPerform:"-" %></td>                
            </tr>
            <%} %>
        </table>
    </div>
</body>
</html>
