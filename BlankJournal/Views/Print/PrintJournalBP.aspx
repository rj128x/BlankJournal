<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="BlankJournal.Models" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>PrintJournalBP</title>
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
            text-align =center;
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
    <%BlankJournal.JournalAnswer answer = Model as BlankJournal.JournalAnswer;  %>
    <%List<JournalRecord> data = answer.Data;
      data.Reverse();
      int index = 0;
      int count = data.Count;
      int countOBP = 0;
      int countTBP = 0;
      foreach (JournalRecord rec in data) {
          if (rec.isOBP)
              countOBP++;
          else countTBP++;
      }
      BlankJournal.BlanksEntities eni = new BlankJournal.BlanksEntities();
      var comments = from c in eni.TBPCommentsTable
                     where (c.Finished == false)
                     select c;
      int commentsActive = comments.Count();
      comments=from c in eni.TBPCommentsTable
               where (c.DateCreate>=answer.dateStart && c.DateCreate<=answer.dateEnd)
               select c;
      int commentsCreate = comments.Count();
    %>
    <div>
        <h1>Журнал переключений с <u><%=answer.dateStart.Value.ToString("dd.MM.yyyy")%></u> по <u><%=answer.dateEnd.Value.ToString("dd.MM.yyyy")%></u></h1>
        <h2><%=String.Format("Всего переключений за период: <u>{0}</u> (по ТБП: <u>{1}</u> по ОБП: <u>{2}</u>)", count, countTBP, countOBP) %></h2>
        <h2><%=String.Format("Активных замечаний: <u>{0}</u> (создано за период <u>{1}</u>)", commentsActive, commentsCreate) %></h2>
        <table>
            <tr>
                <th style="width: 30px;">№пп</th>
                <th style="width: 100px;">Номер</th>
                <th style="width: 70px;">ЛСО</th>
                <th style="width: 120px;">Автор</th>
                <th style="width: 100px;">Основание</th>
                <th>Задание</th>
                <th style="width: 80px;">Дата начала</th>
                <th style="width: 80px;">Дата окончания</th>
                <th>Комментарий</th>
            </tr>

            <%foreach (JournalRecord rec in data) { %>
            <tr>
                <td><%=++index %></td>
                <td><%=rec.ShortNumber %></td>
                <td><%=(rec.isOBP)?(rec.StartLSO+"-"+rec.EndLSO):"-" %></td>
                <td><%=rec.Author %></td>
                <td><%=rec.Zayav %></td>
                <td><%=rec.Task %><%=rec.isOBP?"<br/>Причина ОБП: "+rec.OBPComment:"" %></td>
                <td><%=rec.DateStart.ToString("dd.MM.yyyy") %>
                    <br />
                    <%=rec.DateStart.ToString("HH:mm") %></td>
                <td><%=rec.DateEnd.ToString("dd.MM.yyyy") %><br />
                    <%=rec.DateEnd.ToString("HH:mm") %></td>
                <td><%=rec.Comment %></td>
            </tr>
            <%} %>
        </table>
    </div>
</body>
</html>
