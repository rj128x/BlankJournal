using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class JournalRecord {
		public string Number { get; set; }
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
		public string FileInfoWord { get; set; }
		public string TBPNumber { get; set; }
		public int TBPID { get; set; }
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
			Closed = Finished && DateCreate.AddHours(6) < DateTime.Now;
			isOBP = tbl.isOBP;
			StartLSO = tbl.LSOStart;
			EndLSO = tbl.LSOEnd;
			TBPNumber = tbl.TBPNumber;
			TBPID = tbl.TBPID;
			IDWordData = tbl.WordData;
		}

		public static JournalRecord initTBPRecord(TBPInfo tbp) {
			Logger.info("Инициализация нового ТБП (в журнале)");
			JournalRecord rec = new JournalRecord();
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			DateTime date = DateTime.Now;
			BPJournalTable last = null;
			try {
				last = eni.BPJournalTable.Where(bp => bp.isOBP == false && Math.Truncate(bp.Number) == date.Year).OrderByDescending(bp => bp.Number).First(bp => bp.TBPNumber == tbp.Number);
			} catch { };
			if (last != null) {
				rec.DoubleNumber = last.Number + 0.001;
			} else {
				rec.DoubleNumber = date.Year + 0.001;
			}
			rec.Number = String.Format("ТБП № {0}-{2}/{1}", tbp.Number, Math.Truncate(rec.DoubleNumber), Math.Round((rec.DoubleNumber - date.Year) * 1000));
			rec.Author = DBContext.Single.GetCurrentUser().Login;
			rec.Task = tbp.Name;
			rec.isOBP = false;
			rec.TBPNumber = tbp.Number;
			rec.TBPID = tbp.ID;
			rec.DateCreate = DateTime.Now;
			rec.DateEnd = rec.DateCreate;
			rec.DateStart = rec.DateCreate;
			rec.StartLSO = 0;
			rec.EndLSO = 0;
			rec.Comment = " ";
			Logger.info("Номер ТБП присвоен");
			return rec;
		}

		public static JournalRecord initOBPRecord(TBPInfo tbp) {
			Logger.info("Инициализация нового ОБП (в журнале)");
			JournalRecord rec = new JournalRecord();
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			DateTime date = DateTime.Now;
			BPJournalTable last = null;
			try {
				last = eni.BPJournalTable.Where(bp => bp.isOBP == true && Math.Truncate(bp.Number) == date.Year).OrderByDescending(bp => bp.Number).First();
			} catch { }
			if (last != null) {
				rec.DoubleNumber = last.Number + 0.001;
			} else {
				rec.DoubleNumber = date.Year + 0.001;
			}
			int FullNum = (int)Math.Round((rec.DoubleNumber - date.Year) * 1000);
			rec.Number = String.Format("ОБП № {1}/{0}", Math.Truncate(rec.DoubleNumber), FullNum);
			rec.Author = DBContext.Single.GetCurrentUser().Login;
			rec.Task = tbp.Name;
			rec.isOBP = true;
			rec.TBPNumber = tbp.Number;
			rec.TBPID = tbp.ID;
			if (tbp.Number != "-") {
				if (!String.IsNullOrEmpty(tbp.IDWordData)) {
					try {
						Logger.info("Создание файла ОБП с номером из ТБП");
						string obpFile = BlankJournal.Models.WordData.createOBP(DBContext.TempFolder, tbp, FullNum);
						rec.WordData = File.ReadAllBytes(DBContext.TempFolder + "/" + obpFile);
						rec.FileInfoWord = obpFile;
					} catch (Exception e) {
						Logger.info("Ошибка при создании ОБП из ТБП" + e.ToString());
					}
				}
			} else {
				try {
					Logger.info("Создание пустого файла ОБП с номером из ТБП");
					string obpFile = BlankJournal.Models.WordData.createEmptyOBP(DBContext.TempFolder, FullNum);
					rec.WordData = File.ReadAllBytes(DBContext.TempFolder + "/" + obpFile);
					rec.FileInfoWord = obpFile;
				} catch (Exception e) {
					Logger.info("Ошибка при создании ОБП пустого" + e.ToString());
				}
			}
			rec.DateCreate = DateTime.Now;
			rec.DateEnd = rec.DateCreate;
			rec.DateStart = rec.DateCreate;
			rec.Comment = " ";

			rec.StartLSO = DBContext.Single.MaxLSO + 1;
			rec.EndLSO = DBContext.Single.MaxLSO + 2;

			Logger.info("шаблон ОБП создан");
			return rec;
		}

		public static ReturnMessage CreateBP(JournalRecord record) {
			Logger.info("Создание/изменение зписи о переключении в журнале");
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			BPJournalTable blank = (from b in eni.BPJournalTable where 
												b.Number == record.DoubleNumber && b.TBPNumber == record.TBPNumber && b.isOBP == record.isOBP
											select b).FirstOrDefault();
			if (blank!=null) {
				if (record.isInit)
					return new ReturnMessage(false, "Ошибка при создании бланка переключений. Бланк с такими параметрами уже создан");
			}
			try {
				BPJournalTable tbl = record.isInit ? new BPJournalTable() : blank;
				tbl.Id = record.Number;
				tbl.isOBP = record.isOBP;
				tbl.TBPNumber = record.TBPNumber;
				tbl.TBPID = record.TBPID;
				tbl.Author = DBContext.Single.GetCurrentUser().Login;
				tbl.Comment = record.Comment;
				tbl.Name = record.Task;
				tbl.Number = record.DoubleNumber;
				tbl.DateStart = record.DateStart;
				tbl.DateCreate = record.DateCreate;
				tbl.DateEnd = record.DateEnd;

				if (record.WordData != null && record.isOBP) {
					Logger.info("Загрузка прикрепленного файла ");
					DataTable data = (from d in eni.DataTable where d.ID == record.IDWordData select d).FirstOrDefault();
					if (data == null){
						data=new DataTable();
						data.ID = Guid.NewGuid().ToString();
						eni.DataTable.Add(data);						
					}
					data.DateCreate = DateTime.Now;
					data.Author = DBContext.Single.GetCurrentUser().Login;
					data.FileInfo = record.FileInfoWord;
					data.md5 = MD5Class.getString(record.WordData);
					
					data.Data = record.WordData;
					tbl.WordData = data.ID;
				}

				if (!record.isOBP) {
					Logger.info("Поиск связанного ТБП (для сохранения сылки на файл");
					try {
						TBPInfoTable tbp = (from t in eni.TBPInfoTable where t.Number == tbl.TBPNumber select t).FirstOrDefault();
						if (tbp!=null) {
							tbl.PDFData = tbp.DataPDF;
						} else {
							throw new Exception("Бланк не найден" + tbl.TBPNumber);
						}
					} catch (Exception e) {
						Logger.info("ошибка при формировании записи в журнале переключений. ТБП не найден");
					}
				} else {
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
				return new ReturnMessage(true, "Бланк успешно создан");
			} catch (Exception e) {
				Logger.info("Ошибка при создаии бланка " + e.ToString());
				return new ReturnMessage(false, "Ошибка при создании бланка");
			}

		}
	}
}