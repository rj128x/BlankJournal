using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System.IO;

namespace BlankJournal.Models {
	public class WordData {
		public static string createOBP(string folder, TBPInfo tbp, int num=-1) {
			BlanksEntities eni = new BlanksEntities();
			IQueryable<DataTable> data = from d in eni.DataTable where d.ID == tbp.IDWordData select d;
			if (data.Count() == 0) {
				Logger.info("Файл не найден");
				return null;
			}
			DataTable dt = data.First();
			string fn = "ОБП "+dt.FileInfo;
			string fullpath = folder  + fn;
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

			Run headerRun = new Run(new Text("Бланк перелючений №"+(num>0?num.ToString():"____")));
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

		public static string createEmptyOBP(string folder, int num = -1) {
			string fn = String.Format("ОБП {0}.docx",num);
			string fullpath = folder + fn;
			byte[] data = File.ReadAllBytes( DBContext.DataFolder + "BodyOBP.docx");
			System.IO.File.WriteAllBytes(fullpath, data);
			WordprocessingDocument doc = WordprocessingDocument.Open(fullpath, true);
			Body body = doc.MainDocumentPart.Document.Body;
			IEnumerable<OpenXmlElement> paragraphs = body.Elements<OpenXmlElement>();

			Justification just = new Justification() { Val = JustificationValues.Center };
			RunProperties rPr = new RunProperties();
			rPr.FontSize = new FontSize();
			rPr.FontSize.Val = new StringValue("48");
			rPr.Bold = new Bold();
			rPr.Bold.Val = new OnOffValue(true);

			Run headerRun = new Run(new Text("Бланк перелючений №" + (num > 0 ? num.ToString() : "____")));
			headerRun.RunProperties = rPr;
			Paragraph par = new Paragraph(headerRun);
			par.ParagraphProperties = new ParagraphProperties();
			par.ParagraphProperties.AppendChild(just);
			body.PrependChild(par);

			headerRun = new Run(new Text("Объект переключений: " ));
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
	}
}