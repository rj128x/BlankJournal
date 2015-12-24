using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class JournalRecord {
		public string Number{get;set;}
		public string Task { get; set; }
		public string Comment { get; set; }
		public string Author { get; set; }
		public DateTime DateStart { get; set; }
		public DateTime DateEnd { get; set; }
		public JournalRecord() {
		}

		public JournalRecord(BPJournalTable tbl) {
			Author = DBContext.Single.AllUsers[tbl.Author].Name;

		}
	}
}