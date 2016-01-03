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
			PdfReader reader=new PdfReader(data);
			PdfStamper pdfStamper = new PdfStamper(reader, new FileStream(newFileName, FileMode.Create, FileAccess.Write, FileShare.None));
			Font f = new Font(Font.FontFamily.COURIER,12);
			Phrase p = new Phrase(num, f);
			PdfContentByte over = pdfStamper.GetOverContent(1);

			Rectangle rect = reader.GetPageSizeWithRotation(1);
			float x = (float) (rect.Width * 0.85);
			float y = (float)(rect.Height - rect.Height * 0.25);

			ColumnText.ShowTextAligned(over, Element.ALIGN_LEFT, p, x, y, 0);

			over.SaveState();
			pdfStamper.FormFlattening = true;
			pdfStamper.Close();

		}
	}
}