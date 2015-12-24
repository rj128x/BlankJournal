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
		public double DoubleNumber { get; set; }
		public DateTime DateStart { get; set; }
		public DateTime DateEnd { get; set; }
		public bool isOBP { get; set; }
		public string TBPNumber { get; set; }
		public JournalRecord() {
		}

		public JournalRecord(BPJournalTable tbl) {
			Number = tbl.Id;
			Task = tbl.Name;
			Comment = tbl.Comment;
			Author = DBContext.Single.AllUsers[tbl.Author].Name;
			DoubleNumber = tbl.Number;
			DateStart = tbl.DateStart;
			DateEnd = tbl.DateEnd;
			isOBP = tbl.isOBP;
			TBPNumber = tbl.TBPNumber;
		}

		public static JournalRecord initTBPRecord(TBPInfo tbp) {
			JournalRecord rec = new JournalRecord();
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			DateTime date=DateTime.Now;
			BPJournalTable last = null;
			try {
				last = eni.BPJournalTable.Where(bp => bp.isOBP == false && Math.Truncate(bp.Number) == date.Year).OrderByDescending(bp => bp.Number).First(bp => bp.TBPNumber == tbp.Number);
			}
			catch { };
			if (last != null) {
				rec.DoubleNumber = last.Number + 0.001;				
			}
			else {
				rec.DoubleNumber = date.Year + 0.001;
			}
			rec.Number = String.Format("ТБП № {0}-{2}/{1}", tbp.Number, Math.Truncate(rec.DoubleNumber), Math.Round((rec.DoubleNumber - date.Year) * 1000));
			rec.Author = DBContext.Single.GetCurrentUser().Login;
			rec.Task = tbp.Name;
			rec.isOBP = false;
			rec.TBPNumber = tbp.Number;
			return rec;
		}

		public static JournalRecord initOBPRecord(TBPInfo tbp) {
			JournalRecord rec = new JournalRecord();
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			DateTime date = DateTime.Now;
			BPJournalTable last = null;
			try {
				last = eni.BPJournalTable.Where(bp => bp.isOBP == true && Math.Truncate(bp.Number) == date.Year).OrderByDescending(bp => bp.Number).First();
			}
			catch { }
			if (last != null) {
				rec.DoubleNumber = last.Number + 0.001;
			}
			else {
				rec.DoubleNumber = date.Year + 0.001;
			}
			rec.Number = String.Format("ОБП № {1}/{0}", Math.Truncate(rec.DoubleNumber), Math.Round((rec.DoubleNumber - date.Year) * 1000));
			rec.Author = DBContext.Single.GetCurrentUser().Login;
			rec.Task = tbp.Name;
			rec.isOBP = true;
			rec.TBPNumber = tbp.Number;
			return rec;
		}

		public static ReturnMessage CreateBP(JournalRecord record) {
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			IQueryable<BPJournalTable> blanks = from b in eni.BPJournalTable where b.Number == record.DoubleNumber && b.TBPNumber == record.TBPNumber && b.isOBP==record.isOBP select b;
			if (blanks.Count() > 0) {
				return new ReturnMessage(false, "Ошибка при создании бланка переключений. Бланк с такими параметрами уже создан");
			}
			try {
				BPJournalTable tbl = new BPJournalTable();
				tbl.Id = record.Number;
				tbl.isOBP = record.isOBP;
				tbl.TBPNumber = record.TBPNumber;
				tbl.Author = DBContext.Single.GetCurrentUser().Login;
				tbl.Comment = record.Comment;
				tbl.Name = record.Task;
				tbl.Number = record.DoubleNumber;
				tbl.DateCreate = DateTime.Now;
				tbl.DateStart = DateTime.Now;
				tbl.DateEnd = DateTime.Now;
				eni.BPJournalTable.Add(tbl);
				eni.SaveChanges();
				return new ReturnMessage(true,"Бланк успешно создан");
			}
			catch (Exception e) {
				return new ReturnMessage(false, "Ошибка при создании бланка");
			}
			
		}
	}
}