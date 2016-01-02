using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class TBPInfo  {
		public string Number { get; set; }
		public int ID { get; set; }
		public string FolderID { get; set; }
		public string Name { get; set; }
		public bool Active { get; set; }
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
		public string FileInfoPDF { get; set; }
		public string FileInfoWord { get; set; }
		public string md5Word { get; set; }
		public string md5PDF { get; set; }
		public int CountActiveComments { get; set; }


		public TBPInfo() {

		}

		public TBPInfo(TBPInfoTable tbl) {
			Number = tbl.Number;
			ID = tbl.ID;
			Name = tbl.Name;
			FolderID = tbl.Folder;
			IDPDFData = tbl.DataPDF;
			IDWordData = tbl.DataWord;
			ObjectInfo = tbl.ObjectInfo;
			Active = tbl.isActive;
		}

	}
}