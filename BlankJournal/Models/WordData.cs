using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System.IO;

namespace BlankJournal.Models
{
	public class WordData
	{
		public static int getCountPages(string name) {
			Logger.info("Получение количества страниц документа " + name);
			try {
				WordprocessingDocument doc = WordprocessingDocument.Open(name, false);
				//int pageCount = Int32.Parse(doc.ExtendedFilePropertiesPart.Properties.Pages.Text);
				string str = doc.MainDocumentPart.Document.InnerXml;
				string s0 = "lastRenderedPageBreak";
				int count = (str.Length - str.Replace(s0, "").Length) / s0.Length + 1;
				Logger.info(count.ToString());
				doc.Close();
				//Logger.info(pageCount.ToString());

				return count;
			} catch (Exception e) {
				Logger.info("Ошибка при получении количества страниц документа " + e.ToString());
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
				Logger.info("Абзацев:  " + paragraphs.Count().ToString());
				List<OpenXmlElement> forDel = new List<OpenXmlElement>();
				List<OpenXmlElement> forDelEnd = new List<OpenXmlElement>();
				bool foundCel = false;
				bool foundEnd = false;
				foreach (OpenXmlElement elem in paragraphs) {
					if (!(elem is SectionProperties)) {
						if (!foundCel && !elem.InnerText.ToLower().Contains("цель переключений")) {
							forDel.Add(elem);
						} else {
							foundCel = true;
						}

						string low = elem.InnerText.ToLower();
						if (low.Contains("условия") && low.Contains("применения") && low.Contains("тбп")) {
							try {
								string str = elem.InnerXml;
								int i = str.IndexOf("Т");
								char[] chars = str.ToArray();
								chars[i] = 'О';
								elem.InnerXml = new string(chars);
							} catch (Exception e) {

							}
						}

						if (elem.InnerText.ToLower().Contains("окончание:")) {
							if (foundEnd) {
								forDelEnd = new List<OpenXmlElement>();
							}
							foundEnd = true;
						} else {
							if (foundEnd) {
								forDelEnd.Add(elem);
							}
						}
					}
				}

				Logger.info("Удаление шапки и окончания бланка");
				foreach (OpenXmlElement p in forDel) {
					body.RemoveChild<OpenXmlElement>(p);
				}
				foreach (OpenXmlElement p in forDelEnd) {
					body.RemoveChild<OpenXmlElement>(p);
				}

				WriteHeaderInfo(doc, num,tbp.Name);


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

				WriteHeaderInfo(doc, num, "");

				Logger.info("Добавление новой шапки и окончания");

				WriteRegularData(body, "", num);

				doc.Close();
				return fn;
			} catch (Exception e) {
				Logger.info("ошибка при формировании пустого ОБП " + e.ToString());
				return null;
			}
		}

		protected static void WriteHeaderInfoTop(WordprocessingDocument doc, int num = -1) {
			try {
				Run rn = new Run(new Text(String.Format("ОБП № {0}", num == -1 ? "____" : num.ToString())));
				rn.RunProperties = new RunProperties() {
					FontSize = new FontSize() { Val = new StringValue("25") },
					RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
				};
				Paragraph newPar = new Paragraph(rn);
				newPar.ParagraphProperties = new ParagraphProperties();
				newPar.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Right });

				PageMargin pm = new PageMargin();
				pm.Top = 700;
				pm.Bottom = 900;
				pm.Left = 1200;
				pm.Right = 700;
				pm.Header = 300;
				pm.Footer = 300;

				SectionProperties sp2 = doc.MainDocumentPart.Document.Body.GetFirstChild<SectionProperties>();
				if (sp2 != null) {
					sp2.AppendChild(new HeaderReference() { Id = "obpID", Type = HeaderFooterValues.Default });
					sp2.AppendChild(pm);
				}
				Header header = new Header(newPar);
				HeaderPart hp = doc.MainDocumentPart.AddNewPart<HeaderPart>("obpID");
				header.Save(hp);
			}catch (Exception e) {
				Logger.info("Ошибка при формировании колонтитула "+e.ToString());
			}
		}

		protected static void WriteHeaderInfo(WordprocessingDocument doc, int num = -1, string name="") {
			try {
				string txt = String.Format("ОБП №{0}  -", num == -1 ? "____" : num.ToString());
				Run rn = new Run(new Text(txt));
				rn.RunProperties = new RunProperties() {
					FontSize = new FontSize() { Val = new StringValue("25") },
					RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },					
				};

				PageNumber pn = new PageNumber();
				rn.Append(pn);
				rn.Append(new Text("-"));
				Paragraph newPar = new Paragraph(rn);
				newPar.ParagraphProperties = new ParagraphProperties();
				newPar.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Right });

				rn = new Run(new Text(string.IsNullOrEmpty(name)?"__________________________________":name));
				rn.RunProperties = new RunProperties() {
					FontSize = new FontSize() { Val = new StringValue("20") },
					RunFonts = new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }					
				};
			
				Paragraph newPar2 = new Paragraph(rn);
				newPar2.ParagraphProperties = new ParagraphProperties();
				newPar2.ParagraphProperties.AppendChild(new Justification() { Val = JustificationValues.Left });
				newPar2.ParagraphProperties.AppendChild(new Indentation() { Right=new StringValue("3000") });
				newPar2.ParagraphProperties.AppendChild(new TopBorder() {Val=BorderValues.Single });


				PageMargin pm = new PageMargin();
				pm.Top = 700;
				pm.Bottom = 800;
				pm.Left = 1200;
				pm.Right = 700;
				pm.Header = 300;
				pm.Footer = 500;



				
				//List<FooterPart> fordel = doc.MainDocumentPart.FooterParts.ToList();
				doc.MainDocumentPart.DeleteParts<FooterPart>(doc.MainDocumentPart.FooterParts);

				List<FooterReference>frList= doc.MainDocumentPart.Document.Descendants<FooterReference>().ToList();
				foreach (FooterReference fr in frList) {
					fr.Remove();
				}

				SectionProperties sp2 = doc.MainDocumentPart.Document.Body.GetFirstChild<SectionProperties>();
				if (sp2 != null) {
					sp2.AppendChild(new FooterReference() { Id = "obpID", Type = HeaderFooterValues.Default });
					sp2.AppendChild(pm);
				}

				Footer footer = new Footer(newPar2);
				footer.Append(newPar);
				FooterPart fp = doc.MainDocumentPart.AddNewPart<FooterPart>("obpID");
				footer.Save(fp);
			} catch (Exception e) {
				Logger.info("Ошибка при формировании колонтитула " + e.ToString());
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