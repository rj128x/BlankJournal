using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class PDFClass {
		public static void addTBPNumber(byte[] data, string newFileName, string num) {
			try {
				Logger.info("Добавление номера к pdf файлу " + newFileName + " " + num);
				PdfReader reader = new PdfReader(data);
				BaseFont bf = BaseFont.CreateFont(DBContext.DataFolder+"COURBD.TTF",BaseFont.IDENTITY_H,BaseFont.NOT_EMBEDDED);
				PdfStamper pdfStamper = new PdfStamper(reader, new FileStream(newFileName, FileMode.Create, FileAccess.Write, FileShare.None));
				Font f = new Font(bf, 12);
				Phrase p = new Phrase(num, f);
				float w = bf.GetWidthPoint(num, 12);
				float h = bf.GetAscentPoint(num, 12);
				//for (int i = 1; i <= reader.NumberOfPages; i++) {
					PdfContentByte over = pdfStamper.GetOverContent(1);

					Rectangle rect = reader.GetPageSizeWithRotation(1);
					float x = (float)(rect.Width / 2 - w / 2);
					float y = (float)(rect.Height - h * 2);

					ColumnText.ShowTextAligned(over, Element.ALIGN_LEFT, p, x, y, 0);

					over.SaveState();
				//}
				pdfStamper.FormFlattening = true;
				pdfStamper.Close();
			} catch (Exception e) {
				Logger.info("Ошибка при дбавлении номера к pdf файлу" + e.ToString());
			}
		}
	}
}