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

		public static ReturnMessage RemoveHystoryRecord(TBPHistoryRecord record) {
			try {
				Logger.info(String.Format("Удаление записи из истории редактирования бланка {0} - {1} ({2}) ",record.DateCreate,record.TBPNumber,record.Author));
				BlanksEntities eni = new BlanksEntities();
				TBPHistoryTable tbl=(from h in eni.TBPHistoryTable where h.Id==record.ID select h).FirstOrDefault();

				IQueryable<TBPInfoTable> tbpRef = (from t in eni.TBPInfoTable where t.DataPDF == tbl.NewPDFData || t.DataWord == tbl.NewWordData select t);
				if (tbpRef.Count() > 0) {
					return new ReturnMessage(false, "Ошибка при удалении записи в истории. Активный бланк");
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
				return new ReturnMessage(true, "Запись удалена");
			}
			catch (Exception e) {
				Logger.info("Ошибка при удалении записи в истории " + e.ToString());
				return new ReturnMessage(false, "Ошибка при удалении записи в истории. Возможно есть ссылки на переключения ");
			}
		}
	}
}