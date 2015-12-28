using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Office.Interop.Word;

namespace BlankJournal.Models {
	public class WordData {
		public static string createOBP(string folder, TBPInfo tbp) {
			BlanksEntities eni = new BlanksEntities();
			IQueryable<DataTable> data = from d in eni.DataTable where d.ID == tbp.IDWordData select d;
			if (data.Count() == 0) {
				Logger.info("Файл не найден");
				return null;
			}
			DataTable dt = data.First();
			string fn = dt.ID.ToString() + ".docx";
			string fullpath = folder + fn;
			System.IO.File.WriteAllBytes(fullpath, dt.Data);


			Application app = new Microsoft.Office.Interop.Word.Application();
			Document doc = app.Documents.Open(fullpath);
			bool finish = false;
			foreach (Paragraph p in doc.Range().Paragraphs) {
				if (p.Range.Text.ToLower().Contains("цель переключений")) {
					finish = true;
					break;
				}
				else {
					p.Range.Delete();
					foreach (Table t in p.Range.Tables)
						t.Delete();
				}
			}

			bool found = false;
			foreach (Paragraph p in doc.Range().Paragraphs) {
				if (!found && p.Range.Text.ToLower().Contains("окончание:")) {
					found = true;
					continue;
				}
				if (found) {
					p.Range.Delete();
					foreach (Table t in p.Range.Tables)
						t.Delete();
				}
			}

			Range rng = doc.Range();
			rng.Paragraphs.Add(rng.Paragraphs.First.Range);
			Table tab = rng.Tables.Add(rng.Paragraphs.First.Range, 2, 1);
			tab.Rows[1].Cells[1].Range.Text = "Бланк переключений №__________";
			tab.Rows[1].Cells[1].Range.Font.Size = 16;
			tab.Rows[1].Cells[1].Range.Font.Bold = 2;
			tab.Rows[1].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

			tab.Rows[2].Cells[1].Range.Text = "Объект переключений: Воткинская ГЭС";
			tab.Rows[2].Cells[1].Range.Font.Size = 12;
			tab.Rows[2].Cells[1].Range.Font.Bold = 2;
			tab.Rows[2].Cells[1].Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;


			rng.Paragraphs.Add(rng.Paragraphs.Last.Range);
			tab = rng.Tables.Add(rng.Paragraphs.Last.Range, 6, 2);
			tab.Range.Font.Size = 10;
			tab.Rows[1].Cells[1].Range.Text = "Бланк заполнил и переключение производит:";
			tab.Rows[1].Cells[2].Range.Text = "______________________________________";
			tab.Rows[2].Cells[2].Range.Text = "(должность, ФИО, подпись)";

			tab.Rows[3].Cells[1].Range.Text = "Бланк проверил и переключение контролирует";
			tab.Rows[3].Cells[2].Range.Text = "______________________________________";
			tab.Rows[4].Cells[2].Range.Text = "(должность, ФИО, подпись)";

			tab.Rows[5].Cells[1].Range.Text = "Бланк проверил, переключения разрешаю: ";
			tab.Rows[5].Cells[2].Range.Text = "______________________________________";
			tab.Rows[6].Cells[2].Range.Text = "(должность, ФИО, подпись)";



			doc.Save();
			app.Quit(true);
			return fn;
		}
	}
}