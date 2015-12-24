using System;
using System.Collections.Generic;
using System.Linq;
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
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			IQueryable<UsersTable> users = from u in eni.UsersTable select u;
			AllUsers = new Dictionary<string, User>();
			foreach (UsersTable user in users) {
				AllUsers.Add(user.Login.ToLower(),new User(user));
			}
			IQueryable<FoldersTable> folders = from f in eni.FoldersTable select f;
			AllFolders = new Dictionary<int, Folder>();
			foreach (FoldersTable fld in folders) {
				AllFolders.Add(fld.Id, new Folder(fld));
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
			List<TBPInfo> result = new List<TBPInfo>();

			return result;
		}

		 
	}
}