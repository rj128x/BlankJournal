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
		public static int getCountPages(string name) {
			Logger.info("Получение количества страниц документа " + name);
			try {
				WordprocessingDocument doc = WordprocessingDocument.Open(name, true);
				int pageCount = Int32.Parse(doc.ExtendedFilePropertiesPart.Properties.Pages.Text);
				doc.Close();
				Logger.info(pageCount.ToString());

				return pageCount;
			}
			catch {
				return 1;
			}
		}

		public static string createOBP(string folder, TBPInfo tbp, int num = -1) {
			Logger.info("Формирование ОБП из ТБП " + tbp.Number);
			BlanksEntities eni = new BlanksEntities();
			DataTable dt = (from d in eni.DataTable where d.ID == tbp.IDWordData select d).FirstOrDefault();
			if (dt == null) {
				Logger.info("Файл не найден");
				return null;
			}

			string fn = "ОБП " + dt.FileInfo;
			string fullpath = folder + fn;
			try {
				Logger.info("Формирование временного файла " + fullpath);
				System.IO.File.WriteAllBytes(fullpath, dt.Data);
			} catch (Exception e) {
				Logger.info("ошибка при формировании временног файла " + e.ToString());
				return null;
			}

			try {
				Logger.info("Открытие документа");
				WordprocessingDocument doc = WordprocessingDocument.Open(fullpath, true);
				Body body = doc.MainDocumentPart.Document.Body;
				IEnumerable<OpenXmlElement> paragraphs = body.Elements<OpenXmlElement>();
				Logger.info("Абзацев:  "+paragraphs.Count().ToString());
				List<OpenXmlElement> forDel = new List<OpenXmlElement>();
				bool foundCel = false;
				bool foundEnd = false;
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

				Logger.info("Удаление шапки и окончания бланка");
				foreach (OpenXmlElement p in forDel) {
					body.RemoveChild<OpenXmlElement>(p);
				}

				Logger.info("Добавление новой шапки и окончания");
				WriteRegularData(body, tbp.ObjectInfo, num);
				doc.Close();
				return fn;
			} catch (Exception e) {
				Logger.info("Ошибка при формировании обп" + e.ToString());
				return null;
			}
		}

		public static string createEmptyOBP(string folder, int num = -1) {
			Logger.info("Создание пустого ОБП");
			try {
				Logger.info("Получение шаблона ОБП");
				string fn = String.Format("ОБП {0}.docx", num);
				string fullpath = folder + fn;
				byte[] data = File.ReadAllBytes(DBContext.DataFolder + "BodyOBP.docx");
				Logger.info("Запись шаблона ОБП в файл " + fullpath);
				System.IO.File.WriteAllBytes(fullpath, data);
				WordprocessingDocument doc = WordprocessingDocument.Open(fullpath, true);
				Body body = doc.MainDocumentPart.Document.Body;
				IEnumerable<OpenXmlElement> paragraphs = body.Elements<OpenXmlElement>();
				Logger.info("Добавление новой шапки и окончания");
				WriteRegularData(body, "", num);

				doc.Close();
				return fn;
			} catch (Exception e) {
				Logger.info("ошибка при формировании пустого ОБП "+e.ToString());
				return null;
			}
		}

		protected static void WriteRegularData(Body body, string obj, int num = -1) {

			string str = "Бланк переключений №" + (num > 0 ? num.ToString() : "____");
			Run headerRun = new Run(new Text(str));
			headerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("40") },
				RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
				Bold = new Bold() { Val = new OnOffValue(true) }
			};
			Paragraph par = new Paragraph(headerRun);
			par.ParagraphProperties = new ParagraphProperties();
			par.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });
			body.InsertBefore(par, body.FirstChild);
			
			str = "Объект переключений: Воткинская ГЭС, " + obj;
			headerRun = new Run(new Text(str));
			headerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("30") },
				RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
				Bold = new Bold() { Val = new OnOffValue(true) }
			};
			par = new Paragraph(headerRun);
			par.ParagraphProperties = new ParagraphProperties();			
			par.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Center });
			body.InsertAfter(par, body.FirstChild);

			


			Run footerRun = new Run(new Text("Бланк заполнил и переключение производит:"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("22") },
				RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
			};
			body.AppendChild(new Paragraph(footerRun));

			footerRun = new Run(new Text(" ___________________________________________"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("20") },
				RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
			};
			par = new Paragraph(footerRun);
			par.ParagraphProperties = new ParagraphProperties();
			par.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Right });
			body.AppendChild(par);


			footerRun = new Run(new Text("(должность, ФИО, подпись)"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("20") },
				RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
			};
			par = new Paragraph(footerRun);
			par.ParagraphProperties = new ParagraphProperties();
			par.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Right });
			body.AppendChild(par);

			footerRun = new Run(new Text("Бланк проверил и переключение контролирует:"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("22") },
				RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
			};
			body.AppendChild(new Paragraph(footerRun));

			footerRun = new Run(new Text(" ___________________________________________"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("20") },
				RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
			};
			par = new Paragraph(footerRun);
			par.ParagraphProperties = new ParagraphProperties();
			par.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Right });
			body.AppendChild(par);

			footerRun = new Run(new Text("(должность, ФИО, подпись)"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("20") },
				RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
			};
			par = new Paragraph(footerRun);
			par.ParagraphProperties = new ParagraphProperties();
			par.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Right });
			body.AppendChild(par);

			footerRun = new Run(new Text("Бланк проверил, переключения разрешаю:"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("22") },
				RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
			};
			body.AppendChild(new Paragraph(footerRun));

			footerRun = new Run(new Text(" ___________________________________________"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("20") },
				RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
			};
			par = new Paragraph(footerRun);
			par.ParagraphProperties = new ParagraphProperties();
			par.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Right });
			body.AppendChild(par);

			footerRun = new Run(new Text("(должность, ФИО, подпись)"));
			footerRun.RunProperties = new RunProperties() {
				FontSize = new FontSize() { Val = new StringValue("20") },
				RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
			};
			par = new Paragraph(footerRun);
			par.ParagraphProperties = new ParagraphProperties();
			par.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Right });
			body.AppendChild(par);
		}
	}
}