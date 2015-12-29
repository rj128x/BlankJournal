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
		public DateTime DateCreate { get; set; }
		public bool isOBP { get; set; }
		public bool isInit { get; set; }
		public byte[] WordData { get; set; }
		public string IDWordData { get; set; }
		public string TBPNumber { get; set; }
		public bool Finished { get; set; }
		public bool Closed { get; set; }
		public int StartLSO { get; set; }
		public int EndLSO { get; set; }
		public JournalRecord() {
		}

		public JournalRecord(BPJournalTable tbl) {
			Number = tbl.Id;
			Task = tbl.Name;
			Comment = tbl.Comment;
			Author = DBContext.Single.getUserByLogin(tbl.Author).Name;
			DoubleNumber = tbl.Number;
			DateStart = tbl.DateStart;
			DateEnd = tbl.DateEnd;
			DateCreate = tbl.DateCreate;
			Finished = !(DateStart == DateCreate || DateEnd == DateCreate);
			Closed = Finished && DateCreate.AddDays(1) < DateTime.Now;
			isOBP = tbl.isOBP;
			StartLSO = tbl.LSOStart.HasValue?tbl.LSOStart.Value:0;
			EndLSO = tbl.LSOEnd.HasValue ? tbl.LSOEnd.Value : 0;
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
			rec.DateCreate = DateTime.Now;
			rec.DateEnd = rec.DateCreate;
			rec.DateStart = rec.DateCreate;
			rec.StartLSO = 0;
			rec.EndLSO = 0;

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
			rec.DateCreate = DateTime.Now;
			rec.DateEnd = rec.DateCreate;
			rec.DateStart = rec.DateCreate;

			rec.StartLSO = DBContext.Single.MaxLSO+1;
			rec.EndLSO = DBContext.Single.MaxLSO+2;

			return rec;
		}

		public static ReturnMessage CreateBP(JournalRecord record) {
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			IQueryable<BPJournalTable> blanks = from b in eni.BPJournalTable where b.Number == record.DoubleNumber && b.TBPNumber == record.TBPNumber && b.isOBP==record.isOBP select b;
			BPJournalTable last = null;
			if (blanks.Count() > 0) {
				last = blanks.First();
				if (record.isInit)
					return new ReturnMessage(false, "Ошибка при создании бланка переключений. Бланк с такими параметрами уже создан");
			}
			try {
				BPJournalTable tbl=record.isInit?new BPJournalTable():last;
				tbl.Id = record.Number;
				tbl.isOBP = record.isOBP;
				tbl.TBPNumber = record.TBPNumber;
				tbl.Author = DBContext.Single.GetCurrentUser().Login;
				tbl.Comment = record.Comment;
				tbl.Name = record.Task;
				tbl.Number = record.DoubleNumber;
				tbl.DateStart = record.DateStart;
				tbl.DateCreate = record.DateCreate;
				tbl.DateEnd = record.DateEnd;

				if (record.WordData!=null && record.isOBP) {
					DataTable dat=new DataTable();
					dat.ID = !string.IsNullOrEmpty(record.IDWordData)?record.IDWordData:Guid.NewGuid().ToString();
					dat.DateCreate = DateTime.Now;
					dat.Author = DBContext.Single.GetCurrentUser().Login;
					IQueryable<DataTable> data=from d in eni.DataTable where d.ID==record.IDWordData select d;
					if (data.Count() > 0)
						dat = data.First();
					else
						eni.DataTable.Add(dat);
					dat.Data = record.WordData;
					tbl.WordData = dat.ID;
				}

				if (!record.isOBP) {
					try {						
						IQueryable<TBPInfoTable> tbpList = from t in eni.TBPInfoTable where t.Number == tbl.TBPNumber select t;
						if (tbpList.Count() > 0) {
							TBPInfoTable tbpInfo = tbpList.First();
							tbl.PDFData = tbpInfo.DataPDF;
						} else {
							throw new Exception("Бланк не найден" + tbl.TBPNumber);
						}
					} catch (Exception e) {
						Logger.info("ошибка при формировании записи в журнале переключений. ТБП не найден");
					}
				}
				else {
					tbl.LSOStart = record.StartLSO;
					tbl.LSOEnd = record.EndLSO;
				}
				
				if (record.isInit)
					eni.BPJournalTable.Add(tbl);
				eni.SaveChanges();
				if (record.isOBP) {
					DBContext.Single.MaxLSO = record.EndLSO > DBContext.Single.MaxLSO ? record.EndLSO : DBContext.Single.MaxLSO;
					DBContext.Single.LastOBP = tbl.Id;
				}
				Logger.info("Бланк создан");
				return new ReturnMessage(true, "Бланк успешно создан");
			}
			catch (Exception e) {
				Logger.info("Ошибка при создаии бланка " + e.ToString());
				Logger.info(e.StackTrace);
				return new ReturnMessage(false, "Ошибка при создании бланка");
			}
			
		}
	}
}