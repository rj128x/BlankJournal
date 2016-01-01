using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BlankJournal.Models {
	public class DBContext {
		public static DBContext Single { get; protected set; }
		public static void Init() {
			Single = new DBContext();
			Single.createInitData();
		}

		public Dictionary<string, User> AllUsers;
		public Dictionary<string, Folder> AllFolders;
		public int MaxLSO { get; set; }
		public string LastOBP { get; set; }

		protected void createInitData() {
			Logger.info("Инициализация контекста БД");
			try {
				InitUsers();
				BlankJournal.BlanksEntities eni = new BlanksEntities();

				Logger.info("Чтение папок");
				IQueryable<FoldersTable> folders = from f in eni.FoldersTable where f.Id != "-" select f;
				AllFolders = new Dictionary<string, Folder>();
				foreach (FoldersTable fld in folders) {
					AllFolders.Add(fld.Id, new Folder(fld));
				}

				Logger.info("Чтение ЛСО");
				BPJournalTable last = (from j in eni.BPJournalTable where j.isOBP && j.DateCreate.Year == DateTime.Now.Year orderby j.LSOEnd descending select j).FirstOrDefault();
				try {
					MaxLSO = last.LSOEnd;
				} catch (Exception e) {
					Logger.info("Ошибка при получении максимального номера ЛСО из БД" + e.ToString());
					MaxLSO = 0;
				}

				Logger.info("Чтение ОБП");
				BPJournalTable lastOBP = (from j in eni.BPJournalTable where j.isOBP && j.DateCreate.Year == DateTime.Now.Year orderby j.Number descending select j).FirstOrDefault();
				try {
					LastOBP = lastOBP.Id;
				} catch (Exception e) {
					Logger.info("Ошибка при получении максимального номера ЛСО из БД" + e.ToString());
					LastOBP = "";
				}

			} catch (Exception e) {
				Logger.info("ошибка при инициализации " + e.ToString());
			}
		}

		public void InitUsers() {
			Logger.info("Чтение пользователей");
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			IQueryable<UsersTable> users = from u in eni.UsersTable select u;
			AllUsers = new Dictionary<string, User>();
			foreach (UsersTable user in users) {
				AllUsers.Add(user.Login.ToLower(), new User(user));
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
				//IQueryable<TBPInfoTable> blanks = from b in eni.TBPInfoTable  where b.Folder == folderID select b;
				var blanks = from b in eni.TBPInfoTable
								 from dat in eni.DataTable.Where(dat => dat.ID == b.DataPDF).DefaultIfEmpty()
								 from dat2 in eni.DataTable.Where(dat2 => dat2.ID == b.DataWord).DefaultIfEmpty()
								 where b.Folder == folderID
								 select new { blank = b, FileInfoPDF = dat.FileInfo, FileInfoWord = dat2.FileInfo };
				Dictionary<string, TBPInfo> res = new Dictionary<string, TBPInfo>();
				foreach (var tbl in blanks) {
					TBPInfo tbp = new TBPInfo(tbl.blank);
					tbp.FileInfoPDF = tbl.FileInfoPDF;
					tbp.FileInfoWord = tbl.FileInfoWord;
					res.Add(tbl.blank.Number, tbp);
				}

				IQueryable<BPJournalTable> latest = from j in eni.BPJournalTable orderby j.Number ascending
																where res.Keys.Contains(j.TBPNumber) && j.DateCreate.Year == DateTime.Now.Year && j.isOBP == false
																select j;
				foreach (BPJournalTable bp in latest) {
					try {
						res[bp.TBPNumber].LastOper = bp.DateCreate;
						res[bp.TBPNumber].LastNumber = bp.Id;
						res[bp.TBPNumber].HasLastOper = true;
					} catch (Exception e) {
						Logger.info(e.ToString());
					}
				}

				IQueryable<TBPCommentsTable> comments = from c in eni.TBPCommentsTable
																	 where res.Keys.Contains(c.TBPNumber) && c.Finished == false
																	 select c;
				foreach (TBPCommentsTable com in comments) {
					try {
						res[com.TBPNumber].CountActiveComments++;
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

		public List<JournalRecord> GetJournalBP() {
			Logger.info("Получение журнала переключений ");
			try {
				List<JournalRecord> result = new List<JournalRecord>();
				BlankJournal.BlanksEntities eni = new BlanksEntities();
				var blanks = from b in eni.BPJournalTable
								 from dat in eni.DataTable.Where(dat => b.WordData == dat.ID && b.isOBP == true).DefaultIfEmpty()
								 orderby b.DateCreate descending
								 select new { blank = b, FileInfo = dat.FileInfo };
				foreach (var tbl in blanks) {
					JournalRecord blank = new JournalRecord(tbl.blank);
					blank.FileInfoWord = tbl.FileInfo;
					result.Add(blank);
				}
				return result;
			} catch (Exception e) {
				Logger.info("Ошибка при получении журнала переключений " + e.ToString());
				return new List<JournalRecord>();
			}
		}

		public List<TBPComment> GetCommentsList() {
			Logger.info("Получение списка замечаний ");
			try {
				List<TBPComment> result = new List<TBPComment>();
				BlankJournal.BlanksEntities eni = new BlanksEntities();
				var comments = from c in eni.TBPCommentsTable
									from dat in eni.DataTable.Where(dat => dat.ID == c.WordData).DefaultIfEmpty()
									orderby c.DateCreate descending
									select new { comment = c, Fileinfo = dat.FileInfo };
				foreach (var tbl in comments) {
					TBPComment com = new TBPComment(tbl.comment);
					com.FileInfoData = tbl.Fileinfo;
					result.Add(com);
				}
				return result;
			} catch (Exception e) {
				Logger.info("Ошибка при получении списка замечаний " + e.ToString());
				return new List<TBPComment>();
			}
		}


		public ReturnMessage createTBP(TBPInfo newBlank, bool edit = false) {
			Logger.info("Создание ТБП");
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			TBPInfoTable last = null;

			try {
				IQueryable<TBPInfoTable> exist = from b in eni.TBPInfoTable where b.Number == newBlank.Number select b;
				if (exist.Count() > 0) {
					last = exist.First();
					if (!edit) {
						Logger.info("Бланк не создан - дубль");
						return new ReturnMessage(false, String.Format("Бланк с номером {0} уже существует", newBlank.Number));
					}
				}
			} catch (Exception e) {
				Logger.info("Ошибка при создании бланка " + e.ToString());
				return new ReturnMessage(false, String.Format("Ошибка при создании бланка"));
			}
			try {
				TBPInfoTable tbl = edit ? last : new TBPInfoTable();
				tbl.Number = newBlank.Number;
				tbl.Name = newBlank.Name;
				tbl.ObjectInfo = newBlank.ObjectInfo;
				tbl.Folder = newBlank.FolderID;
				if (!edit)
					eni.TBPInfoTable.Add(tbl);
				if (newBlank.UpdatedPDF || newBlank.UpdatedWord)
					SaveTBPDataToDB(newBlank, tbl, eni);
				eni.SaveChanges();
			} catch (Exception e) {
				Logger.info("Ошибка при создании бланка " + e.ToString());
				return new ReturnMessage(false, "Ошибка при создании бланка " + e.ToString());
			}
			return new ReturnMessage(true, "Бланк успешно создан");
		}

		public List<TBPHistoryRecord> getTBPHistory(TBPInfo tbp) {
			Logger.info("Получение истории изменения ТБП");
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			try {
				List<TBPHistoryRecord> result = new List<TBPHistoryRecord>();
				IQueryable<TBPHistoryTable> list = from h in eni.TBPHistoryTable where h.TBPNumber == tbp.Number orderby h.DateCreate descending select h;
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
			try {
				TBPHistoryTable hist = new TBPHistoryTable();
				hist.Id = Guid.NewGuid().ToString();
				hist.Author = GetCurrentUser().Login;
				hist.DateCreate = DateTime.Now;
				
				if (newBlank.UpdatedWord) {
					hist.PrevWordData = tbl.DataWord;					
					DataTable word = new DataTable();
					word.ID = Guid.NewGuid().ToString();
					word.Author = GetCurrentUser().Login;
					word.DateCreate = DateTime.Now;
					hist.NewWordData = word.ID;
					tbl.DataWord = word.ID;
					word.Data = newBlank.WordData;
					word.FileInfo = newBlank.FileInfoWord;
					eni.DataTable.Add(word);
				}

				if (newBlank.UpdatedPDF) {
					hist.PrevPDFData = tbl.DataPDF;
					DataTable pdf = new DataTable();
					pdf.ID = Guid.NewGuid().ToString();
					pdf.Author = GetCurrentUser().Login;
					pdf.DateCreate = DateTime.Now;
					hist.NewPDFData = pdf.ID;
					tbl.DataPDF = pdf.ID;
					pdf.isPDF = true;
					pdf.Data = newBlank.PDFData;
					pdf.FileInfo = newBlank.FileInfoPDF;
					eni.DataTable.Add(pdf);

				}

				hist.TBPNumber = tbl.Number;

				eni.TBPHistoryTable.Add(hist);
			} catch (Exception e) {
				Logger.info("Ошиба при записи файлов в БД " + e.ToString());
				return false;
			}
			return true;
		}

	}
}