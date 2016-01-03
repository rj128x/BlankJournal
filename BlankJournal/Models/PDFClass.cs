using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class PDFClass {
		public static void addTBPNumber(byte[] data, string newFileName, string num) {
			PdfReader reader=new PdfReader(data);
			PdfStamper pdfStamper = new PdfStamper(reader, new FileStream(newFileName, FileMode.Create, FileAccess.Write, FileShare.None));
			Image bmp = new Bitmap(200, 20);
			Graphics g = Graphics.FromImage(bmp);
			g.DrawString(num, new Font("Times New Roman", 10), Brushes.Black, new PointF(0, 0));
			g.Save();
			iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(bmp, iTextSharp.text.BaseColor.WHITE);

			PdfDictionary pg=reader.GetPageN(1);
			iTextSharp.text.Rectangle rect = reader.GetPageSizeWithRotation(pg);
			int x = (int)(rect.Width * 0.85);
			int y = (int)(rect.Height * 0.25);
			img.SetAbsolutePosition(0, rect.Height-20);
			PdfContentByte waterMark;
			//    
				waterMark = pdfStamper.GetOverContent(1);
				waterMark.AddImage(img);
			//
			pdfStamper.FormFlattening = true;
			pdfStamper.Close();

		}
	}
}