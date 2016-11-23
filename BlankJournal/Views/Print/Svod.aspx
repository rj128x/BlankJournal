<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="BlankJournal.Models" %>
<!DOCTYPE html>

<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>Статистика</title>
    <style>
        table, tr, td, th, p {
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

            table td, table th {
                border-width: 1px;
                border-style: solid;
                border-color: cornflowerblue;
                padding-left: 2px;
                padding-right: 2px;
                text-align: center;
                font-family: 'Arial';
                font-size: 8pt;
            }

        th {
            text-align: center;
        }

            td.center, th.center {
                text-align: center;
            }

            td.left, th.left {
                text-align: left;
            }

            td.right, th.right {
                text-align: right;
            }


        table td.right, table th.right {
            text-align: right;
        }

        table td.fill, table th.fill {
            background-color: Gray;
        }

        table th.cap {
            width: 60px;
            min-width: 60px;
            max-width: 60px;
        }

        table td.tbp, table th.tbp {
            background-color: lightgreen;
            width: 60px;
            min-width: 60px;
            max-width: 60px;
        }

        table td.tbp {
            text-align: left;
        }

        table td.obp, table th.obp {
            background-color: lightblue;
            width: 60px;
            min-width: 60px;
            max-width: 60px;
        }

        table td.obp {
            text-align: right;
        }

        table td.bb table th.bb {
             border-bottom-width:7px;
        }

       
        table td.tb table th.tb {

            border-bottom-width:7px;
        }
    </style>
</head>
<body>
    <%BlankJournal.Models.SvodReport Svod = Model as BlankJournal.Models.SvodReport;  %>
    <h1>Таблица учета Бланков переключений за <%=Svod.Year%> год</h1>
    <table>
        <tr >
            <th style="border-top-width:3px; border-top-color:darkred;" colspan="3" class="fill tb bb">&nbsp;</th>            

            <th style="border-top-width:3px; border-top-color:darkred;" class="cap fill">Сумма</th>
            <th style="border-top-width:3px; border-top-color:darkred;" class="cap fill">Январь</th>
            <th style="border-top-width:3px; border-top-color:darkred;" class="cap fill">Февраль</th>
            <th style="border-top-width:3px; border-top-color:darkred;" class="cap fill">Март</th>
            <th style="border-top-width:3px; border-top-color:darkred;" class="cap fill">Апрель</th>
            <th style="border-top-width:3px; border-top-color:darkred;" class="cap fill">Май</th>
            <th style="border-top-width:3px; border-top-color:darkred;" class="cap fill">Июнь</th>
            <th style="border-top-width:3px; border-top-color:darkred;" class="cap fill">Июль</th>
            <th style="border-top-width:3px; border-top-color:darkred;" class="cap fill">Август</th>
            <th style="border-top-width:3px; border-top-color:darkred;" class="cap fill">Сентябрь</th>
            <th style="border-top-width:3px; border-top-color:darkred;" class="cap fill">Октябрь</th>
            <th style="border-top-width:3px; border-top-color:darkred;" class="cap fill">Ноябрь</th>
            <th style="border-top-width:3px; border-top-color:darkred;" class="cap fill">Декабрь</th>
            
        </tr>
        <tr>
            <th style="border-bottom-width: 3px; border-bottom-color:darkred; border-top-width:3px; border-top-color:darkred;" colspan="16" class="fill">
                Переключения<br />
                * - ОБП созданные из пустого шаблона (не на основе ТБП)
            </th>
        </tr>
        <tr>
            <th rowspan="4" style="border-bottom-width: 3px; border-top-width:3px; border-bottom-color:darkred;border-top-color:darkred;">Все папки</th>
            <th>Замечания</th>     
            <th>Сумма</th>                   
            <th><%=Svod.SumInfo.Sum %></th>
            <%for (int month = 1; month <= 12; month++) { %>
            <td><%=SvodReport.getText(Svod.SumInfo.MonthInfo[month])%></td>            
            <%} %>            
        </tr>
        <tr>
            <th rowspan="3" style="border-bottom-width: 3px; border-bottom-color:darkred; color:red"><%=Svod.CommentActiveInfo.Sum %></th>
            <th class="tbp">ТБП</th>
            <th class="tbp"><%=Svod.TBPInfo.Sum %></th>
            <%for (int month = 1; month <= 12; month++) { %>
            <td class="tbp"><%=SvodReport.getText(Svod.TBPInfo.MonthInfo[month]) %></td>
            <%} %>            
        </tr>
        <tr>
            <th class="obp">ОБП</th>
            <th class="obp"><%=Svod.OBPInfo.Sum %></th>
            <%for (int month = 1; month <= 12; month++) { %>
            <td class="obp"><%=SvodReport.getText(Svod.OBPInfo.MonthInfo[month]) %></td>
            <%} %>
        </tr>
        <tr>
            <th  style="border-bottom-width: 3px; border-bottom-color:darkred;" class="obp">ОБП * </th>
            <th  style="border-bottom-width: 3px; border-bottom-color:darkred;"  class="obp"><%=Svod.OBPEmptyInfo.Sum %></th>
            <%for (int month = 1; month <= 12; month++) { %>
            <td  style="border-bottom-width: 3px; border-bottom-color:darkred;"  class="obp"><%=SvodReport.getText(Svod.OBPEmptyInfo.MonthInfo[month]) %></td>
            <%} %>
        </tr>
        <%foreach (string folder in DBContext.Single.AllFolders.Keys) {
              if (folder == "del") continue;%>
        <tr>
            <th style="border-bottom-width: 3px; border-bottom-color:darkred;" rowspan="3" class="bb"><%=folder%><br />
                <%=DBContext.Single.AllFolders[folder].Name %></th>
            <th>Замечания</th>
            <th>Сумма</th>            
            <th class="center"><%=Svod.OBPFoldersInfo[folder].Sum+Svod.TBPFoldersInfo[folder].Sum %></th>
            <%for (int month = 1; month <= 12; month++) { %>
            <td class="center"><%=SvodReport.getText(Svod.OBPFoldersInfo[folder].MonthInfo[month]+Svod.TBPFoldersInfo[folder].MonthInfo[month]) %></td>
            <%} %>            
        </tr>
        <tr>
             <th style="border-bottom-width: 3px; border-bottom-color:darkred; color:red;" rowspan="2" class="bb" ><%=Svod.CommentActiveFoldersInfo[folder].MonthInfo.Last().Value %></th>
            <th class="tbp">ТБП</th>
            <th class="tbp"><%=Svod.TBPFoldersInfo[folder].Sum %></th>
            <%for (int month = 1; month <= 12; month++) { %>
            <td class="tbp"><%=SvodReport.getText(Svod.TBPFoldersInfo[folder].MonthInfo[month]) %></td>
            <%} %>           
        </tr>
        <tr>
            <th  style="border-bottom-width: 3px; border-bottom-color:darkred;"  class="obp">ОБП</th>
            <th  style="border-bottom-width: 3px; border-bottom-color:darkred;"  class="obp"><%=Svod.OBPFoldersInfo[folder].Sum %></th>
            <%for (int month = 1; month <= 12; month++) { %>
            <td  style="border-bottom-width: 3px; border-bottom-color:darkred;"  class="obp"><%=SvodReport.getText(Svod.OBPFoldersInfo[folder].MonthInfo[month]) %></td>
            <%} %>
        </tr>

        <%} %>
        <tr>
            <th style="border-bottom-width: 3px; border-top-width:3px; border-bottom-color:darkred;border-top-color:darkred;" colspan="16" class="fill tb bb">
                Замечания<br />
                * - количество активных (не закрытых) замечаний на конец месяца
            </th>
        </tr>
        <tr>
            <th colspan="3">Создано замечаний</th>
            <th class="left"><%=Svod.CommentInfo.Sum %></th>
            <%for (int month = 1; month <= 12; month++) { %>
            <td class="left"><%=SvodReport.getText(Svod.CommentInfo.MonthInfo[month]) %></td>
            <%} %>
        </tr>
        <tr>
            <th colspan="3">Устранено замечаний</th>
            <th class="right"><%=Svod.CommentUstrInfo.Sum %></th>
            <%for (int month = 1; month <= 12; month++) { %>
            <td class="right"><%=SvodReport.getText(Svod.CommentUstrInfo.MonthInfo[month]) %></td>
            <%} %>
        </tr>
        <tr>
            <th  style="border-bottom-width: 3px; border-bottom-color:darkred;"  colspan="3">Активных замечаний *</th>
            <th  style="border-bottom-width: 3px; border-bottom-color:darkred;"  class="center"><%=Svod.CommentActiveInfo.Sum %></th>
            <%for (int month = 1; month <= 12; month++) { %>
            <td  style="border-bottom-width: 3px; border-bottom-color:darkred;"  class="center"><%=SvodReport.getText(Svod.CommentActiveInfo.MonthInfo[month]) %></td>
            <%} %>
        </tr>

    </table>

</body>
</html>
