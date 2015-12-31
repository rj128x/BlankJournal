using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class TBPInfo {
		public string Number { get; set; }
		public string FolderID { get; set; }
		public string Name { get; set; }
		public byte[] WordData { get; set; }
		public byte[] PDFData { get; set; }
		public string IDPDFData { get; set; }
		public string IDWordData { get; set; }
		public bool UpdatedPDF { get; set; }
		public bool UpdatedWord { get; set; }
		public bool EditingTBP { get; set; }
		public DateTime LastOper { get; set; }
		public String LastNumber { get; set; }
		public bool HasLastOper { get; set; }
		public string ObjectInfo { get; set; }


		public TBPInfo() {

		}

		public TBPInfo(TBPInfoTable tbl) {
			Number = tbl.Number;
			Name = tbl.Name;
			FolderID = tbl.Folder;
			IDPDFData = tbl.DataPDF;
			IDWordData = tbl.DataWord;
			ObjectInfo = tbl.ObjectInfo;
		}
	}
}