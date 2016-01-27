using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class JournalRecord {
		public string Number { get; set; }
		public string ShortNumber { get; set; }
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
		public bool Started { get; set; }
		public bool Closed { get; set; }
		public int StartLSO { get; set; }
		public int EndLSO { get; set; }

		public bool HasCrossDate { get; set; }
		public bool HasCrossLSO { get; set; }

		public string CrossDate { get; set; }
		public string CrossLSO { get; set; }

		public static double MAX_BP_YEAR = 1000.0;

		public JournalRecord() {
		}

		public JournalRecord(BPJournalTable tbl) {
			Number = tbl.Id;
			ShortNumber = tbl.IDShort;
			Task = tbl.Name;
			Comment = tbl.Comment;
			Author = DBContext.Single.getUserByLogin(tbl.Author).Name;
			DoubleNumber = tbl.Number;
			DateStart = tbl.DateStart.HasValue ? tbl.DateStart.Value : tbl.DateCreate;
			DateEnd = tbl.DateEnd.HasValue ? tbl.DateEnd.Value : tbl.DateCreate;
			DateCreate = tbl.DateCreate;
			Finished = tbl.Finished;
			Started = tbl.Started;
			Closed = Started && Finished && tbl.LastUpdateFinish.HasValue && (tbl.LastUpdateFinish.Value.AddHours(6) < DateTime.Now);
			//Logger.info(tbl.LastUpdateFinish.ToString() + " " + Closed.ToString());
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
			}
			catch { };
			if (last != null) {
				rec.DoubleNumber = last.Number + 0.001;
			}
			else {
				rec.DoubleNumber = date.Year + 0.001;
			}
			rec.Number = String.Format("ТБП № {0}/{2}/{1}", tbp.Number, Math.Truncate(rec.DoubleNumber), Math.Ceiling((rec.DoubleNumber - date.Year) * MAX_BP_YEAR));
			rec.ShortNumber = String.Format("ТБП № {0}/{1}", tbp.Number, Math.Ceiling((rec.DoubleNumber - date.Year) * MAX_BP_YEAR));
			rec.Author = DBContext.Single.GetCurrentUser().Login;
			rec.Task = tbp.Name;
			rec.isOBP = false;
			rec.TBPNumber = tbp.Number;
			rec.TBPID = tbp.ID;
			DateTime dt = DateTime.Now;
			rec.DateCreate = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0,DateTimeKind.Unspecified);
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
			}
			catch { }
			if (last != null) {
				rec.DoubleNumber = last.Number + 1 / MAX_BP_YEAR;
			}
			else {
				rec.DoubleNumber = date.Year + 1 / MAX_BP_YEAR;
			}
			int FullNum = (int)Math.Ceiling((rec.DoubleNumber - date.Year) * MAX_BP_YEAR);
			rec.Number = String.Format("ОБП № {1}/{0}", Math.Truncate(rec.DoubleNumber), FullNum);
			rec.ShortNumber = String.Format("ОБП № {0}", FullNum);
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
					}
					catch (Exception e) {
						Logger.info("Ошибка при создании ОБП из ТБП" + e.ToString());
					}
				}
			}
			else {
				try {
					Logger.info("Создание пустого файла ОБП с номером из ТБП");
					string obpFile = BlankJournal.Models.WordData.createEmptyOBP(DBContext.TempFolder, FullNum);
					rec.WordData = File.ReadAllBytes(DBContext.TempFolder + "/" + obpFile);
					rec.FileInfoWord = obpFile;
				}
				catch (Exception e) {
					Logger.info("Ошибка при создании ОБП пустого" + e.ToString());
				}
			}
			DateTime dt=DateTime.Now;
			rec.DateCreate = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0,DateTimeKind.Unspecified);
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
			string addMessage = "";
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			BPJournalTable blank = (from b in eni.BPJournalTable
															where
																b.Number == record.DoubleNumber && b.TBPNumber == record.TBPNumber && b.isOBP == record.isOBP
															select b).FirstOrDefault();
			if (blank != null) {
				if (record.isInit)
					return new ReturnMessage(false, "Ошибка при создании бланка переключений. Бланк с такими параметрами уже создан");
			}
			try {
				if (record.Finished && !record.Started)
					record.Finished = false;
				if (record.Started && record.Finished) {
					if (record.DateStart > record.DateEnd)
						return new ReturnMessage(false, "Дата окончания переключений больше даты начала. Создание бланка невозможно");
				}

				BPJournalTable tbl = record.isInit ? new BPJournalTable() : blank;
				tbl.Id = record.Number;
				tbl.IDShort = record.ShortNumber;
				tbl.isOBP = record.isOBP;
				tbl.TBPNumber = record.TBPNumber;
				tbl.TBPID = record.TBPID;
				tbl.Author = DBContext.Single.GetCurrentUser().Login;
				tbl.Comment = record.Comment;
				tbl.Name = record.Task;
				tbl.Number = record.DoubleNumber;
				if (record.Started)
					tbl.DateStart = record.DateStart;
				else
					tbl.DateStart = null;

				if (record.Finished) {
					tbl.DateEnd = record.DateEnd;
					tbl.LastUpdateFinish = DateTime.Now;
				}
				else {
					tbl.DateEnd = null;
					tbl.LastUpdateFinish = null;
				}
				tbl.DateCreate = record.DateCreate;

				tbl.Started = record.Started;
				tbl.Finished = record.Finished;

				addMessage = checkCrossData(record);

				if (record.WordData != null && record.isOBP) {
					Logger.info("Загрузка прикрепленного файла ");
					DataTable data = (from d in eni.DataTable where d.ID == record.IDWordData select d).FirstOrDefault();
					if (data == null) {
						data = new DataTable();
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
						if (tbp != null) {
							tbl.PDFData = tbp.DataPDF;
						}
						else {
							throw new Exception("Бланк не найден" + tbl.TBPNumber);
						}
					}
					catch (Exception e) {
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
					DBContext.Single.RezLSO = DBContext.Single.MaxLSO + 1;
					DBContext.Single.LastOBP = tbl.IDShort;
					double num = (tbl.Number + 1 / MAX_BP_YEAR - tbl.DateCreate.Year) * MAX_BP_YEAR;
					DBContext.Single.RezOBP = string.Format("ОБП № {0}", (int)Math.Ceiling(num));
				}
				return new ReturnMessage(true, record.isInit ? "Бланк успешно создан\n" + addMessage : "Бланк успешно изменен\n" + addMessage);
			}
			catch (Exception e) {
				Logger.info("Ошибка при создаии бланка " + e.ToString());
				return new ReturnMessage(false, record.isInit ? "Ошибка при создании бланка" : "Ошибка при изменении бланка");
			}

		}

		public static string checkCrossData(JournalRecord record) {
			string addMessage = "";
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			if (record.Started || record.Finished) {
				IQueryable<BPJournalTable> crossData = from b in eni.BPJournalTable
																							 where
																									(b.Id != record.Number) &&
																								 ((record.Started && b.Started && record.DateStart >= b.DateStart && record.DateStart <= b.DateEnd) ||
																								 (record.Finished && b.Finished && record.DateEnd <= b.DateEnd && record.DateEnd >= b.DateStart) ||
																								 (record.Started && record.Finished && record.DateStart <= b.DateStart && record.DateEnd >= b.DateEnd) ||
																								 (record.Started && record.Finished && record.DateStart >= b.DateStart && record.DateEnd <= b.DateEnd))
																							 select b;
				if (crossData.Count() > 0) {
					List<string> numbers = new List<string>();
					List<string> numbersShort = new List<string>();
					foreach (BPJournalTable cross in crossData) {
						numbers.Add(String.Format("{0} ({1} - {2})", cross.IDShort,
							(cross.Started ? cross.DateStart.Value.ToString("dd.MM.yyyy HH:mm") : "???"),
							(cross.Finished ? cross.DateEnd.Value.ToString("dd.MM.yyyy HH:mm") : "???")));
						numbersShort.Add(cross.IDShort);
					}
					addMessage = "\n Внимание: Данный бланк пересекается по времени с бланками: " + String.Join(", ", numbers);
					record.HasCrossDate = true;
					record.CrossDate = String.Join(", ", numbersShort);
				}
			}

			if (record.isOBP) {
				IQueryable<BPJournalTable> crossData = from b in eni.BPJournalTable
																							 where
																								(b.Id != record.Number && b.isOBP && b.DateCreate.Year == record.DateCreate.Year) &&
																								 (
																								 (record.StartLSO >= b.LSOStart && record.StartLSO <= b.LSOEnd) ||
																								 (record.EndLSO >= b.LSOStart && record.EndLSO <= b.LSOEnd) ||
																								 (record.StartLSO <= b.LSOStart && record.EndLSO >= b.LSOEnd)
																								 )
																							 select b;
				if (crossData.Count() > 0) {
					List<string> numbers = new List<string>();
					List<string> numbersShort = new List<string>();
					foreach (BPJournalTable cross in crossData) {
						numbers.Add(String.Format("{0} ({1} - {2})", cross.IDShort, cross.LSOStart, cross.LSOEnd));
						numbersShort.Add(cross.IDShort);
					}
					addMessage += "\n Внимание: Данный бланк пересекается по ЛСО: " + String.Join(", ", numbers);
					record.HasCrossLSO = true;
					record.CrossLSO = String.Join(", ", numbersShort);
				}
			}
			return addMessage;
		}



	}
}