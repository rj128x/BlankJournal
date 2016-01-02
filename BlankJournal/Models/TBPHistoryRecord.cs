using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class TBPHistoryRecord {
		public string TBPNumber { get; set; }
		public int TBPID { get; set; }
		public string PrevPDFID { get; set; }
		public string PrevWordID { get; set; }
		public string NewPDFID { get; set; }
		public string NewWordID { get; set; }
		public bool HasPrevPDFData { get; set; }
		public bool HasPrevWordData { get; set; }
		public bool HasNewPDFData { get; set; }
		public bool HasNewWordData { get; set; }
		public bool UpdatedWord { get; set; }
		public bool UpdatedPDF { get; set; }
		public string Author { get; set; }
		public DateTime DateCreate { get; set; }

		public TBPHistoryRecord() {

		}

		public TBPHistoryRecord(TBPHistoryTable tbl) {
			TBPNumber = tbl.TBPNumber;
			TBPID = tbl.TBPID;
			PrevPDFID = tbl.PrevPDFData;
			PrevWordID = tbl.PrevWordData;
			NewPDFID = tbl.NewPDFData;
			NewWordID = tbl.NewWordData;
			Author = DBContext.Single.getUserByLogin(tbl.Author).Name;
			DateCreate = tbl.DateCreate;
			HasNewPDFData = !string.IsNullOrEmpty(NewPDFID);
			HasNewWordData = !string.IsNullOrEmpty(NewWordID);
			HasPrevPDFData = !string.IsNullOrEmpty(PrevPDFID);
			HasPrevWordData = !string.IsNullOrEmpty(PrevWordID);
			UpdatedPDF = NewPDFID != PrevPDFID;
			UpdatedWord = NewWordID != PrevWordID;
		}
	}
}