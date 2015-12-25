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
				IQueryable<TBPInfoTable> blanks = from b in eni.TBPInfoTable where b.Folder == folderID select b;
				foreach (TBPInfoTable tbl in blanks) {
					result.Add(new TBPInfo(tbl));
				}
				return result;
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
				IQueryable<BPJournalTable> blanks = from b in eni.BPJournalTable select b;
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


		public ReturnMessage createTBP(TBPInfo newBlank) {
			Logger.info("Создание ТБП");
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			try {				
				IQueryable<TBPInfoTable> exist = from b in eni.TBPInfoTable where b.Number == newBlank.Number select b;
				if (exist.Count() > 0) {
					Logger.info("Бланк не создан - дубль");
					return new ReturnMessage(false, String.Format("Бланк с номером {0} уже существует", newBlank.Number));
				}
			}
			catch (Exception e) {
				Logger.info("Ошибка при создании бланка " + e.ToString());
				return new ReturnMessage(false, String.Format("Ошибка при создании бланка"));
			}
			try {
				TBPInfoTable tbl = new TBPInfoTable();
				tbl.Number = newBlank.Number;
				tbl.Name = newBlank.Name;
				tbl.Folder = newBlank.FolderID;
				eni.TBPInfoTable.Add(tbl);
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
			try {
				DataTable pdf = new DataTable();
				DataTable word = new DataTable();
				TBPHistoryTable hist = new TBPHistoryTable();

				pdf.ID = Guid.NewGuid().ToString();
				word.ID = Guid.NewGuid().ToString();
				hist.Id = Guid.NewGuid().ToString();

				hist.Author = GetCurrentUser().Login;
				pdf.Author = GetCurrentUser().Login;
				word.Author = GetCurrentUser().Login;

				pdf.DateCreate = DateTime.Now;
				word.DateCreate = DateTime.Now;
				hist.DateCreate = DateTime.Now;

				hist.PrevPDFData = tbl.DataPDF;
				hist.PrevWordData = tbl.DataWord;
				hist.NewPDFData = pdf.ID;
				hist.NewWordData = word.ID;
				hist.TBPNumber = tbl.Number;
				
				pdf.isPDF = true;

				pdf.Data = Encoding.Unicode.GetBytes(newBlank.PDFData);
				word.Data = Encoding.Unicode.GetBytes(newBlank.WordData);
				eni.DataTable.Add(pdf);
				eni.DataTable.Add(word);
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