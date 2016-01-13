<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="BlankJournal.Models" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>PrintJournalBP</title>
    <style>
	 	table,tr,td,p {
			font-family: 'Arial';
			font-size: 10pt;
		}
		
		h1,h2,h3,h4,h5,h6,hr
		{
			padding:5;
			margin:5;
		}
		
		
		h1
		{
			font-family: 'Arial';
			font-size: 14pt;
		}
		
		h2
		{
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

        th{
            text-align: center;	
        }

        td.center{
            text-align=center;
        }
               
		
		
		table td.right,table th.right{
			text-align: right;
		}	
		
		table td.fill, table th.fill{
			background-color:Gray;
		}	

	 </style>
</head>
<body>
    <%BlankJournal.JournalAnswer answer = Model as BlankJournal.JournalAnswer;  %>
    <div>
        <h1>Журнал переключений с <%=answer.dateStart.Value.ToString("dd.MM.yyyy")%> по <%=answer.dateStart.Value.ToString("dd.MM.yyyy")%></h1>
        <table>
            <tr>
                <th>Номер</th>
                <th>ЛСО</th>
                <th>Дата создания</th>
                <th>Автор</th>
                <th>Задание</th>
                <th>Дата начала</th>
                <th>Дата окончания</th>
                <th>Комментарий</th>
            </tr>
            <%foreach (JournalRecord rec in answer.Data){ %>
            <tr>
                <td><%=rec.ShortNumber %></td>
                <td><%=(rec.isOBP)?(rec.StartLSO+"-"+rec.EndLSO):"-" %></td>
                <td><%=rec.DateCreate.ToString("dd.MM.yyyy HH:mm") %></td>
                <td><%=rec.Author %></td>
                <td><%=rec.Task %></td>
                <td><%=rec.DateStart.ToString("dd.MM.yyyy HH:mm") %></td>
                <td><%=rec.DateEnd.ToString("dd.MM.yyyy HH:mm") %></td>
                <td><%=rec.Comment %></td>
            </tr>
            <%} %>
        </table>
    </div>
</body>
</html>
