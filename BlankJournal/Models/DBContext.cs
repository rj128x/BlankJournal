using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace BlankJournal.Models
{
	public class DBContext
	{
		public static DBContext Single { get; protected set; }
		public static String TempFolder { get; set; }
		public static String DataFolder { get; set; }
		public static void Init() {
			Single = new DBContext();
			Single.createInitData();
		}

		public Dictionary<string, User> AllUsers;
		public Dictionary<string, Folder> AllFolders;
		public int MaxLSO { get; set; }
		public int RezLSO { get; set; }
		public string LastOBP { get; set; }
		public string RezOBP { get; set; }

		protected void createInitData() {
			Logger.info("Инициализация контекста БД");
			try {

				BlankJournal.BlanksEntities eni = new BlanksEntities();

				Logger.info("Чтение папок");
				IQueryable<FoldersTable> folders = from f in eni.FoldersTable where f.Id != "-" select f;
				SortedList<double, FoldersTable> sorted = new SortedList<double, FoldersTable>();
				foreach (FoldersTable fld in folders) {
					double id = 0;
					try {
						id = Double.Parse(fld.Id);
					} catch {
						id = Double.Parse(fld.Id.Replace(".", ","));
					}
					sorted.Add(id, fld);

				}

				AllFolders = new Dictionary<string, Folder>();
				foreach (FoldersTable fld in sorted.Values) {
					AllFolders.Add(fld.Id, new Folder(fld));
				}
				InitUsers();

				INIT_LSO_OBP();

			} catch (Exception e) {
				Logger.info("ошибка при инициализации " + e.ToString());
			}
		}

		public void INIT_LSO_OBP() {
			Logger.info("Чтение ЛСО");
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			BPJournalTable last = (from j in eni.BPJournalTable where j.isOBP && j.DateCreate.Year == DateTime.Now.Year orderby j.LSOEnd descending select j).FirstOrDefault();
			try {
				MaxLSO = last.LSOEnd;
				RezLSO = MaxLSO + 1;
			} catch (Exception e) {
				Logger.info("Ошибка при получении максимального номера ЛСО из БД" + e.ToString());
				MaxLSO = 0;
				RezLSO = MaxLSO + 1;
			}

			Logger.info("Чтение ОБП");
			BPJournalTable lastOBP = (from j in eni.BPJournalTable where j.isOBP && j.DateCreate.Year == DateTime.Now.Year orderby j.Number descending select j).FirstOrDefault();
			try {
				LastOBP = lastOBP.IDShort;
				double num = (lastOBP.Number + 1 / JournalRecord.MAX_BP_YEAR - lastOBP.DateCreate.Year) * JournalRecord.MAX_BP_YEAR;
				RezOBP = string.Format("ОБП № {0}", (int)Math.Ceiling(num));
			} catch (Exception e) {
				Logger.info("Ошибка при получении максимального номера ОБП из БД" + e.ToString());
				LastOBP = "";
				RezOBP = "ОБП № 1";
			}
		}

		public void InitUsers() {
			Logger.info("Чтение пользователей");
			try {
				BlankJournal.BlanksEntities eni = new BlanksEntities();
				IQueryable<UsersTable> users = from u in eni.UsersTable select u;
				AllUsers = new Dictionary<string, User>();
				foreach (UsersTable user in users) {
					AllUsers.Add(user.Login.ToLower(), new User(user));
				}
			} catch (Exception e) {
				Logger.info("Ошибка при чтении списка пользователей");
			}
		}

		public User getUserByLogin(string login) {
			if (AllUsers.ContainsKey(login.ToLower()))
				return AllUsers[login.ToLower()];
			else {
				Logger.info("Ошибка при поиске пользователя " + login);
				return new User();
			}
		}

		public User GetCurrentUser() {
			string login = HttpContext.Current.User.Identity.Name.ToLower();
			if (AllUsers.ContainsKey(login)) {
				return AllUsers[login];
			} else {
				User user = new User();
				user.Login = login;
				user.Name = "Вход не выполнен: " + login;
				return user;
			}
		}

		public List<TBPInfo> GetTBPListByFolder(string folderID) {
			Logger.info("Получение бланков папки " + folderID);
			try {
				List<TBPInfo> result = new List<TBPInfo>();
				BlankJournal.BlanksEntities eni = new BlanksEntities();
				var blanks = from b in eni.TBPInfoTable
										 from dat in eni.DataTable.Where(dat => dat.ID == b.DataPDF).DefaultIfEmpty()
										 from dat2 in eni.DataTable.Where(dat2 => dat2.ID == b.DataWord).DefaultIfEmpty()
										 where b.Folder == folderID && b.isActive
										 orderby b.Number
										 select new {
											 blank = b,
											 FileInfoPDF = dat.FileInfo,
											 FileInfoWord = dat2.FileInfo,
											 AuthorPDF = dat.Author,
											 AuthorWord = dat2.Author,
											 DatePDF = dat.DateCreate.ToString(),
											 DateWord = dat2.DateCreate.ToString(),
											 md5PDF = dat.md5,
											 md5Word = dat2.md5
										 };
				Dictionary<int, TBPInfo> res = new Dictionary<int, TBPInfo>();
				Dictionary<int, TBPInfo> resID = new Dictionary<int, TBPInfo>();
				foreach (var tbl in blanks) {
					TBPInfo tbp = new TBPInfo(tbl.blank);
					tbp.FileInfoPDF = tbl.FileInfoPDF;
					tbp.FileInfoWord = tbl.FileInfoWord;
					if (!String.IsNullOrEmpty(tbp.FileInfoPDF)) {
						tbp.AuthorPDF = DBContext.Single.getUserByLogin(tbl.AuthorPDF).Name;
						tbp.DatePDF = DateTime.Parse(tbl.DatePDF);
					}
					if (!string.IsNullOrEmpty(tbp.FileInfoWord)) {
						tbp.AuthorWord = DBContext.Single.getUserByLogin(tbl.AuthorWord).Name;
						tbp.DateWord = DateTime.Parse(tbl.DateWord);
					}

					try {
						tbp.WordAfterPDF = tbp.DatePDF.AddDays(1) < tbp.DateWord;
					} catch { }

					tbp.md5PDF = tbl.md5PDF;
					tbp.md5Word = tbl.md5Word;
					res.Add(tbl.blank.ID, tbp);
					resID.Add(tbl.blank.ID, tbp);

				}

				/*IQueryable<BPJournalTable> latest = from j in eni.BPJournalTable
																						orderby j.Number ascending
																						where res.Keys.Contains(j.TBPNumber) && j.DateCreate.Year == DateTime.Now.Year && j.isOBP == false
																						select j;
				foreach (BPJournalTable bp in latest) {
					try {
						res[bp.TBPNumber].LastOper = bp.DateCreate;
						res[bp.TBPNumber].LastNumber = bp.IDShort;
						res[bp.TBPNumber].HasLastOper = true;
					} catch (Exception e) {
						Logger.info(e.ToString());
					}
				}*/

				IQueryable<BPJournalTable> countOper = from j in eni.BPJournalTable
																							 orderby j.Number ascending
																							 where res.Keys.Contains(j.TBPID) && j.DateCreate.Year == DateTime.Now.Year
																							 select j;
				foreach (BPJournalTable bp in countOper) {
					try {
						if (bp.isOBP) {
							res[bp.TBPID].CountOBP++;
						} else {
							res[bp.TBPID].CountTBP++;
						}
					} catch (Exception e) {
						Logger.info(e.ToString());
					}
				}

				IQueryable<TBPCommentsTable> comments = from c in eni.TBPCommentsTable
																								where resID.Keys.Contains(c.TBPID) && c.Finished == false
																								select c;
				foreach (TBPCommentsTable com in comments) {
					try {
						resID[com.TBPID].CountActiveComments++;
					} catch (Exception e) {
						Logger.info(e.ToString());
					}
				}


				return res.Values.ToList();
			} catch (Exception e) {
				Logger.info("Ошибка при получении списка бланков " + e.ToString());
				return new List<TBPInfo>();
			}
		}

		public JournalAnswer GetJournalBP(JournalAnswer Filter) {
			Logger.info("Получение журнала переключений ");
			if (Filter == null) {
				Filter = new JournalAnswer();
				Filter.tbpID = -1;
			}
			if (!Filter.dateStart.HasValue)
				Filter.dateStart = DateTime.Now.Date.AddDays(-20);
			if (!Filter.dateEnd.HasValue)
				Filter.dateEnd = DateTime.Now.Date.AddDays(1);
			if (!string.IsNullOrEmpty(Filter.tbpInfo))
				Filter.tbpID = -1;
			if (Filter.tbpID != -1)
				Filter.tbpInfo = "";
			try {
				List<JournalRecord> result = new List<JournalRecord>();
				BlankJournal.BlanksEntities eni = new BlanksEntities();

				var blanks =
								 from b in eni.BPJournalTable
								 from dat in eni.DataTable.Where(dat => b.WordData == dat.ID && b.isOBP == true).DefaultIfEmpty()
								 where
								 ((b.DateCreate > Filter.dateStart && b.DateCreate < Filter.dateEnd) ||
									(b.Started && b.DateStart > Filter.dateStart && b.DateStart < Filter.dateEnd) ||
									(b.Finished && b.DateEnd > Filter.dateStart && b.DateEnd < Filter.dateEnd))
									&&
									(
									  (Filter.tbpID == -1 && string.IsNullOrEmpty(Filter.tbpInfo))
									   ||
									  (!string.IsNullOrEmpty(Filter.tbpInfo) &&
										(b.TBPNumber.ToLower().Contains(Filter.tbpInfo.ToLower())||
											b.Comment.ToLower().Contains(Filter.tbpInfo.ToLower()) || b.Name.ToLower().Contains(Filter.tbpInfo.ToLower())||
											b.Zayav.ToLower().Contains(Filter.tbpInfo.ToLower())|| b.Id.ToLower().Contains(Filter.tbpInfo.ToLower())))
									   ||
								    (Filter.tbpID != -1 && b.TBPID == Filter.tbpID)
								  )
								 orderby b.Started, b.DateStart descending
								 select new { blank = b, FileInfo = dat.FileInfo };

				foreach (var tbl in blanks) {
					JournalRecord blank = new JournalRecord(tbl.blank);
					blank.FileInfoWord = tbl.FileInfo;
					if (Filter.checkCrossData) {
						JournalRecord.checkCrossData(blank);
					}
					result.Add(blank);
				}
				Filter.Data = result;
				return Filter;
			} catch (Exception e) {
				Logger.info("Ошибка при получении журнала переключений " + e.ToString());
				return Filter;
			}
		}

		public CommentAnswer GetCommentsList(CommentAnswer Filter) {
			Logger.info("Получение списка замечаний ");
			if (Filter == null) {
				Filter = new CommentAnswer();
				Filter.onlyActive = true;
			}
			if (!Filter.dateStart.HasValue)
				Filter.dateStart = DateTime.Now.Date.AddDays(-60);
			if (!Filter.dateEnd.HasValue)
				Filter.dateEnd = DateTime.Now.Date.AddDays(1);

			try {
				List<TBPComment> result = new List<TBPComment>();
				BlankJournal.BlanksEntities eni = new BlanksEntities();
				var comments = from c in eni.TBPCommentsTable
											 from dat in eni.DataTable.Where(dat => dat.ID == c.WordData).DefaultIfEmpty()
											 where (Filter.onlyActive && c.Finished == false &&
														 (string.IsNullOrEmpty(Filter.TBPNumber) || (!string.IsNullOrEmpty(Filter.TBPNumber) && c.TBPNumber == Filter.TBPNumber)))
												||
											 (c.DateCreate > Filter.dateStart && c.DateCreate < Filter.dateEnd && Filter.onlyActive == false)
											 orderby c.DateCreate descending
											 select new { comment = c, Fileinfo = dat.FileInfo };
				foreach (var tbl in comments) {
					TBPComment com = new TBPComment(tbl.comment);
					com.FileInfoData = tbl.Fileinfo;
					result.Add(com);
				}
				Filter.Data = result;
				return Filter;
			} catch (Exception e) {
				Logger.info("Ошибка при получении списка замечаний " + e.ToString());
				return new CommentAnswer();
			}
		}


		public ReturnMessage createTBP(TBPInfo newBlank, bool edit = false) {
			Logger.info(edit ? "Редактирование ТБП" : "Создание ТБП");
			Logger.info(String.Format("ID={0} Number={1}", newBlank.ID, newBlank.Number));
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			TBPInfoTable blank = null;
			bool saveDB = false;
			try {
				blank = (from b in eni.TBPInfoTable where b.Number == newBlank.Number && b.isActive select b).FirstOrDefault();
				if (!edit && blank != null) {
					return new ReturnMessage(false, String.Format("Бланк с номером {0} уже существует", newBlank.Number));
				}
				TBPInfoTable tbl = edit ? (from b in eni.TBPInfoTable where b.ID == newBlank.ID select b).FirstOrDefault() : new TBPInfoTable();
				if (!edit) {
					Logger.info("Поиск номера нового бланка");
					int newID = 0;
					TBPInfoTable maxBlank = (from b in eni.TBPInfoTable orderby b.ID descending select b).FirstOrDefault();
					if (maxBlank != null)
						newID = maxBlank.ID + 1;
					tbl.ID = newID;
					newBlank.ID = newID;
					eni.TBPInfoTable.Add(tbl);
					Logger.info("присвое номер " + newID);
				}
				tbl.Number = newBlank.Number;
				tbl.Name = newBlank.Name;
				tbl.ObjectInfo = newBlank.ObjectInfo;
				tbl.Folder = newBlank.FolderID;
				tbl.canUseTBP = newBlank.CanUseTBP;
				tbl.isActive = true;

				string md5PDF = ""; string md5Word = "";
				try {
					md5PDF = MD5Class.getString(newBlank.PDFData);
				} catch { }
				try {
					md5Word = MD5Class.getString(newBlank.WordData);
				} catch { };

				newBlank.UpdatedPDF = newBlank.UpdatedPDF && newBlank.md5PDF != md5PDF;
				newBlank.UpdatedWord = newBlank.UpdatedWord && newBlank.md5Word != md5Word;
				newBlank.md5PDF = newBlank.UpdatedPDF ? md5PDF : newBlank.md5PDF;
				newBlank.md5Word = newBlank.UpdatedWord ? md5Word : newBlank.md5Word;

				if (newBlank.UpdatedPDF || newBlank.UpdatedWord)
					SaveTBPDataToDB(newBlank, tbl, eni);
				eni.SaveChanges();
				saveDB = FileSync.SyncTBP(tbl);
			} catch (Exception e) {
				Logger.info("Ошибка при создании бланка " + e.ToString());
				return new ReturnMessage(false, "Ошибка при создании бланка ");
			}

			return new ReturnMessage(true, String.Format("Бланк успешно создан.\n Синхронизация в папку: {0}", saveDB ? "Успешно" : "Ошибка"));
		}

		public List<TBPHistoryRecord> getTBPHistory(TBPInfo tbp) {
			Logger.info("Получение истории изменения ТБП " + tbp.Number);
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			try {
				List<TBPHistoryRecord> result = new List<TBPHistoryRecord>();
				IQueryable<TBPHistoryTable> list = from h in eni.TBPHistoryTable where h.TBPID == tbp.ID orderby h.DateCreate descending select h;
				foreach (TBPHistoryTable tbl in list) {
					TBPHistoryRecord rec = new TBPHistoryRecord(tbl);
					result.Add(rec);
				}
				return result;
			} catch (Exception e) {
				Logger.info("Ошибка при получении истории изменения ТБП" + e.ToString());
				return new List<TBPHistoryRecord>();
			}
		}

		public bool SaveTBPDataToDB(TBPInfo newBlank, TBPInfoTable tbl, BlanksEntities eni) {
			Logger.info("Сохранение загруженных данных бланка в БД");
			try {
				TBPHistoryTable hist = new TBPHistoryTable();
				hist.Id = Guid.NewGuid().ToString();
				hist.Author = GetCurrentUser().Login;
				hist.DateCreate = DateTime.Now;

				if (newBlank.UpdatedWord) {
					DataTable word = new DataTable();
					word.ID = Guid.NewGuid().ToString();
					word.Author = GetCurrentUser().Login;
					word.DateCreate = DateTime.Now;
					hist.NewWordData = word.ID;
					tbl.DataWord = word.ID;
					word.Data = newBlank.WordData;
					word.FileInfo = newBlank.FileInfoWord;
					word.md5 = newBlank.md5Word;
					eni.DataTable.Add(word);
				}

				if (newBlank.UpdatedPDF) {
					DataTable pdf = new DataTable();
					pdf.ID = Guid.NewGuid().ToString();
					pdf.Author = GetCurrentUser().Login;
					pdf.DateCreate = DateTime.Now;
					hist.NewPDFData = pdf.ID;
					tbl.DataPDF = pdf.ID;
					pdf.isPDF = true;
					pdf.Data = newBlank.PDFData;
					pdf.FileInfo = newBlank.FileInfoPDF;
					pdf.md5 = newBlank.md5PDF;
					eni.DataTable.Add(pdf);

				}

				hist.TBPNumber = tbl.Number;
				hist.TBPID = tbl.ID;

				eni.TBPHistoryTable.Add(hist);
				Logger.info("Данные успешно сохранены");
			} catch (Exception e) {
				Logger.info("Ошиба при записи файлов в БД " + e.ToString());
				return false;
			}
			return true;
		}

		public ReturnMessage removeTBP(TBPInfo tbp) {
			Logger.info("Удаление бланка " + tbp.Number);
			try {
				BlanksEntities eni = new BlanksEntities();
				TBPInfoTable tbl = (from t in eni.TBPInfoTable where t.ID == tbp.ID select t).FirstOrDefault();
				if (tbl != null) {
					tbl.isActive = false;
					eni.SaveChanges();
					return new ReturnMessage(true, "Бланк удален");
				} else {
					return new ReturnMessage(true, "Бланк не найден");
				}
			} catch (Exception e) {
				Logger.info("ошибка при удалении бланка " + e.ToString());
				return new ReturnMessage(false, "Ошибка при удалении бланка");
			}
		}

	}
}