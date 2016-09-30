using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class TBPHistoryRecord {
		public string TBPNumber { get; set; }
		public string ID { get; set; }
		public int TBPID { get; set; }
		public string PrevPDFID { get; set; }
		public string PrevWordID { get; set; }
		public string NewPDFID { get; set; }
		public string NewWordID { get; set; }
		public bool HasNewPDFData { get; set; }
		public bool HasNewWordData { get; set; }
		public string Author { get; set; }
		public DateTime DateCreate { get; set; }

		public TBPHistoryRecord() {

		}

		public TBPHistoryRecord(TBPHistoryTable tbl) {
			ID = tbl.Id;
			TBPNumber = tbl.TBPNumber;
			TBPID = tbl.TBPID;
			NewPDFID = tbl.NewPDFData;
			NewWordID = tbl.NewWordData;
			Author = DBContext.Single.getUserByLogin(tbl.Author).Name;
			DateCreate = tbl.DateCreate;
			HasNewPDFData = !string.IsNullOrEmpty(NewPDFID);
			HasNewWordData = !string.IsNullOrEmpty(NewWordID);
		}

		public static List<string> ClearHystory() {
			List<string> result = new List<string>();
			if (!DBContext.Single.GetCurrentUser().CanEditTBP) {
				result.Add("У вас нет прав для очистки истории редактирования");
				return result;
			}
			
			try {
				
				Logger.info(String.Format("Удаление записи из истории редактирования бланков"));
				BlanksEntities eni = new BlanksEntities();
				IQueryable<TBPHistoryTable> data = from h in eni.TBPHistoryTable select h;
				int count=0;
				foreach (TBPHistoryTable tbl in data) {
					TBPHistoryRecord rec = new TBPHistoryRecord(tbl);
					ReturnMessage ret=RemoveHystoryRecord(rec);
					if (ret.Result)
						count++;
					result.Add(ret.Message);
				}
				result.Add("История редактирования успешно очищена. Удалено записей: " + count);
				return result;
			}
			catch (Exception e) {
				result.Add("Ошибка при очистке истори");
				return result;
			}

		}

		public static ReturnMessage RemoveHystoryRecord(TBPHistoryRecord record) {
			string info=(String.Format("Удаление записи из истории редактирования бланка {0} - {1} ({2}) ",record.DateCreate,record.TBPNumber,record.Author));
			try {
				
				Logger.info(info);
				BlanksEntities eni = new BlanksEntities();
				TBPHistoryTable tbl=(from h in eni.TBPHistoryTable where h.Id==record.ID select h).FirstOrDefault();


				if (!string.IsNullOrEmpty(tbl.NewPDFData)) {
					IQueryable<TBPInfoTable> tbpRef = (from t in eni.TBPInfoTable where t.ID == record.TBPID && t.DataPDF == tbl.NewPDFData  select t);
					if (tbpRef.Count() > 0) {
						return new ReturnMessage(false, info+" Ошибка при удалении записи в истории. Активный бланк (PDF)");
					}
				}

				if (!string.IsNullOrEmpty(tbl.NewWordData)) {
					IQueryable<TBPInfoTable> tbpRef = (from t in eni.TBPInfoTable where t.ID == record.TBPID && t.DataWord == tbl.NewWordData select t);
					if (tbpRef.Count() > 0) {
						return new ReturnMessage(false, info+" Ошибка при удалении записи в истории. Активный бланк (Word)");
					}
				}


				IQueryable<BPJournalTable> bpRef = (from b in eni.BPJournalTable where !b.isOBP && b.PDFData==tbl.NewPDFData select b);
				if (bpRef.Count() > 0) {
					return new ReturnMessage(false, info+" Ошибка при удалении записи в истории. Есть записи в журнале переключений");
				}


				eni.TBPHistoryTable.Remove(tbl);

				if (!string.IsNullOrEmpty(tbl.NewPDFData)) {
					try {
						DataTable pdf=(from d in eni.DataTable where d.ID==tbl.NewPDFData select d).FirstOrDefault();
						eni.DataTable.Remove(pdf);
					}catch (Exception e){
						Logger.info(e.ToString());
					}
				}

				if (!string.IsNullOrEmpty(tbl.NewWordData)) {
					try {
						DataTable pdf=(from d in eni.DataTable where d.ID==tbl.NewWordData select d).FirstOrDefault();
						eni.DataTable.Remove(pdf);
					}catch (Exception e){
						Logger.info(e.ToString());
					}
				}

				
				eni.SaveChanges();
				return new ReturnMessage(true, info+" Запись удалена");
			}
			catch (Exception e) {
				Logger.info("Ошибка при удалении записи в истории " + e.ToString());
				return new ReturnMessage(false, info+" Ошибка при удалении записи в истории. Возможно есть ссылки на переключения ");
			}
		}
	}
}