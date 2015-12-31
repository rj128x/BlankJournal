using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

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
			string fn = Guid.NewGuid().ToString() + ".docx";
			string fullpath = folder + fn;
			System.IO.File.WriteAllBytes(fullpath, dt.Data);
			WordprocessingDocument doc = WordprocessingDocument.Open(fullpath,true);
			Body body = doc.MainDocumentPart.Document.Body;
			IEnumerable<OpenXmlElement> paragraphs = body.Elements<OpenXmlElement>();
			Logger.info(paragraphs.Count().ToString());
			List<OpenXmlElement> forDel = new List<OpenXmlElement>();
			bool foundCel=false;
			bool foundEnd=false;
			foreach (OpenXmlElement elem in paragraphs) {
				if (!foundCel && !elem.InnerText.ToLower().Contains("цель переключений")) {
					forDel.Add(elem);
				} else {
					foundCel = true;
				}

				if (elem.InnerText.ToLower().Contains("окончание:")) {
					foundEnd = true;
				} else {
					if (foundEnd) {
						forDel.Add(elem);
					}
				}
			}

	
			foreach (OpenXmlElement p in forDel) {
				body.RemoveChild<OpenXmlElement>(p);
			}

			Justification just = new Justification() { Val = JustificationValues.Center };
			RunProperties rPr = new RunProperties();
			rPr.FontSize = new FontSize();
			rPr.FontSize.Val = new StringValue("48");
			rPr.Bold = new Bold();
			rPr.Bold.Val = new OnOffValue(true);

			Run headerRun = new Run(new Text("Бланк перелючений №____"));
			headerRun.RunProperties = rPr;
			Paragraph par = new Paragraph(headerRun);
			par.ParagraphProperties = new ParagraphProperties();
			par.ParagraphProperties.AppendChild(just);
			body.PrependChild(par);

			headerRun = new Run(new Text("Объект переключений: " + tbp.ObjectInfo));
			rPr = new RunProperties();
			rPr.FontSize = new FontSize();
			rPr.FontSize.Val = new StringValue("32");
			rPr.Bold = new Bold();
			rPr.Bold.Val = new OnOffValue(true);
			headerRun.AppendChild(rPr);
			par = new Paragraph(headerRun);
			par.ParagraphProperties = new ParagraphProperties();
			par.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });
			body.InsertAfter(par, body.FirstChild);

			Run footerRun = new Run(new Text("Бланк заполнил и переключение производит:   ___________________________________"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("20") }
			};
			body.AppendChild(new Paragraph(footerRun));

			footerRun = new Run(new Text("(должность, ФИО, подпись)"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("15") }
			};
			par = new Paragraph(footerRun);
			par.ParagraphProperties = new ParagraphProperties();
			par.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Right });
			body.AppendChild(par);

			footerRun = new Run(new Text("Бланк проверил и переключение контролирует: ___________________________________"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("20") }
			};
			body.AppendChild(new Paragraph(footerRun));

			footerRun = new Run(new Text("(должность, ФИО, подпись)"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("15") }
			};
			par = new Paragraph(footerRun);
			par.ParagraphProperties = new ParagraphProperties();
			par.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Right });
			body.AppendChild(par);

			footerRun = new Run(new Text("Бланк проверил, переключения разрешаю:      ___________________________________"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("20") }
			};
			body.AppendChild(new Paragraph(footerRun));

			footerRun = new Run(new Text("(должность, ФИО, подпись)"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("15") }
			};
			par = new Paragraph(footerRun);
			par.ParagraphProperties = new ParagraphProperties();
			par.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Right });
			body.AppendChild(par);

			
			doc.Close();
			return fn;
		}
		/*public static string createOBP(string folder, TBPInfo tbp) {
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

			tab.Rows[2].Cells[1].Range.Text = "Объект переключений: Воткинская ГЭС, "+tbp.ObjectInfo;
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
		}*/
	}
}