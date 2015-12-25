using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class TBPInfo {
		public string Number { get; set; }
		public int FolderID { get; set; }
		public string Name { get; set; }
		public string WordData { get; set; }
		public string PDFData { get; set; }

		public TBPInfo() {

		}

		public TBPInfo(TBPInfoTable tbl) {
			Number = tbl.Number;
			Name = tbl.Name;
			FolderID = tbl.Folder;			
		}
	}
}