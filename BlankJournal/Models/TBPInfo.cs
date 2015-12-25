using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class TBPInfo {
		public string Number { get; set; }
		public int FolderID { get; set; }
		public string Name { get; set; }
		public byte[] WordData { get; set; }
		public byte[] PDFData { get; set; }
		public string IDPDFData { get; set; }
		public string IDWordData { get; set; }

		public TBPInfo() {

		}

		public TBPInfo(TBPInfoTable tbl) {
			Number = tbl.Number;
			Name = tbl.Name;
			FolderID = tbl.Folder;
			IDPDFData = tbl.DataPDF;
			IDWordData = tbl.DataWord;
		}
	}
}