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
		public Dictionary<int, Folder> AllFolders;

		protected void createInitData() {
			Logger.info("Инициализация контекста БД");
			try {
				Logger.info("Чтение пользователей");
				BlankJournal.BlanksEntities eni = new BlanksEntities();
				IQueryable<UsersTable> users = from u in eni.UsersTable select u;
				AllUsers = new Dictionary<string, User>();
				foreach (UsersTable user in users) {
					AllUsers.Add(user.Login.ToLower(), new User(user));
				}
				Logger.info("Чтение папок");
				IQueryable<FoldersTable> folders = from f in eni.FoldersTable where f.Id > 0 select f;
				AllFolders = new Dictionary<int, Folder>();
				foreach (FoldersTable fld in folders) {
					AllFolders.Add(fld.Id, new Folder(fld));
				}
			}
			catch (Exception e) {
				Logger.info("ошибка при инициализации " + e.ToString());
			}
		}

		public User getUserByLogin(string login){
			if (AllUsers.ContainsKey(login.ToLower()))
				return AllUsers[login.ToLower()];
			else{
				Logger.info("Ошибка при поиске пользователя "+login);
				return new User();
			}
		}

		public User GetCurrentUser() {
			string login = HttpContext.Current.User.Identity.Name.ToLower() ;
			if (AllUsers.ContainsKey(login)) {
				return AllUsers[login];
			} else {
				User user = new User();
				user.Login = login;
				user.Name = "Вход не выполнен: " + login;
				return user;
			}
		}

		public List<TBPInfo> GetTBPListByFolder(int folderID) {
			Logger.info("Получение бланков папки " + folderID);
			try {
				List<TBPInfo> result = new List<TBPInfo>();
				BlankJournal.BlanksEntities eni = new BlanksEntities();
				IQueryable<TBPInfoTable> blanks = from b in eni.TBPInfoTable  where b.Folder == folderID select b;
				Dictionary<string, TBPInfo> res = new Dictionary<string, TBPInfo>();
				foreach (TBPInfoTable tbl in blanks) {
					res.Add(tbl.Number,new TBPInfo(tbl));
				}

				IQueryable<BPJournalTable> latest = from j in eni.BPJournalTable orderby j.Number ascending 
																where res.Keys.Contains(j.TBPNumber) && j.DateCreate.Year == DateTime.Now.Year && j.isOBP==false
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
				return res.Values.ToList();
			}
			catch (Exception e) {
				Logger.info("Ошибка при получении списка бланков " + e.ToString());
				return new List<TBPInfo>();
			}
		}

		public List<JournalRecord> GetJournalBP() {
			Logger.info("Получение журнала переключений ");
			try {
				List<JournalRecord> result = new List<JournalRecord>();
				BlankJournal.BlanksEntities eni = new BlanksEntities();
				IQueryable<BPJournalTable> blanks = from b in eni.BPJournalTable orderby b.DateCreate descending select b ;
				foreach (BPJournalTable tbl in blanks) {
					result.Add(new JournalRecord(tbl));
				}
				return result;
			}
			catch (Exception e) {
				Logger.info("Ошибка при получении журнала переключений " + e.ToString());
				return new List<JournalRecord>();
			}
		}

		public List<TBPComment> GetCommentsList() {
			Logger.info("Получение списка замечаний ");
			try {
				List<TBPComment> result = new List<TBPComment>();
				BlankJournal.BlanksEntities eni = new BlanksEntities();
				IQueryable<TBPCommentsTable> comments = from c in eni.TBPCommentsTable orderby c.DateCreate descending select c;
				foreach (TBPCommentsTable tbl in comments) {
					result.Add(new TBPComment(tbl));
				}
				return result;
			} catch (Exception e) {
				Logger.info("Ошибка при получении списка замечаний " + e.ToString());
				return new List<TBPComment>();
			}
		}


		public ReturnMessage createTBP(TBPInfo newBlank,bool edit=false) {
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
				TBPInfoTable tbl = edit?last:new TBPInfoTable();
				tbl.Number = newBlank.Number;
				tbl.Name = newBlank.Name;
				tbl.Folder = newBlank.FolderID;
				if (!edit)
					eni.TBPInfoTable.Add(tbl);
				if (newBlank.UpdatedPDF||newBlank.UpdatedWord)
					SaveTBPDataToDB(newBlank, tbl, eni);
				eni.SaveChanges();
			}
			catch (Exception e) {
				Logger.info("Ошибка при создании бланка " + e.ToString());
				return new ReturnMessage(false, "Ошибка при создании бланка " + e.ToString());				
			}
			return new ReturnMessage(true, "Бланк успешно создан");
		}

		public bool SaveTBPDataToDB(TBPInfo newBlank, TBPInfoTable tbl, BlanksEntities eni) {
				try{
				TBPHistoryTable hist = new TBPHistoryTable();
				hist.Id = Guid.NewGuid().ToString();
				hist.Author = GetCurrentUser().Login;
				hist.DateCreate = DateTime.Now;

				hist.PrevPDFData = tbl.DataPDF;
				hist.PrevWordData = tbl.DataWord;

				if (newBlank.UpdatedWord) {
					DataTable word = new DataTable();
					word.ID = Guid.NewGuid().ToString();
					word.Author = GetCurrentUser().Login;
					word.DateCreate = DateTime.Now;
					hist.NewWordData = word.ID;
					tbl.DataWord = word.ID;
					word.Data = newBlank.WordData;
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
					eni.DataTable.Add(pdf);

				}
				
				hist.TBPNumber = tbl.Number;			
				
				eni.TBPHistoryTable.Add(hist);
			}
			catch (Exception e) {
				Logger.info("Ошиба при записи файлов в БД " + e.ToString());
				return false;
			}
			return true;
		}
		 
	}
}