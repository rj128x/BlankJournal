using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class TBPHistoryRecord {
		public string TBPNumber { get; set; }
		public string PrevPDFID { get; set; }
		public string PrewWordID { get; set; }
		public string NewPDFID { get; set; }
		public string NewWordID { get; set; }
		public string Author { get; set; }
		public DateTime DateCreate { get; set; }

		public TBPHistoryRecord() {

		}

		public TBPHistoryRecord(TBPHistoryTable tbl) {
			TBPNumber = tbl.TBPNumber;
			PrevPDFID = tbl.PrevPDFData;
			PrewWordID = tbl.PrevWordData;
			NewPDFID = tbl.NewPDFData;
			NewWordID = tbl.NewWordData;
			Author = DBContext.Single.getUserByLogin(tbl.Author).Name;
			DateCreate = tbl.DateCreate;
		}
	}
}