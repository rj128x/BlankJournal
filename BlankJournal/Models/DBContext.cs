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

		public Dictionary<string, UsersTable> AllUsers;

		protected void createInitData() {
			BlankJournal.BlanksEntities eni = new BlanksEntities();
			IQueryable<UsersTable> users = eni.UsersTable.Where(usr => true);
			AllUsers = new Dictionary<string, UsersTable>();
			foreach (UsersTable user in users) {
				AllUsers.Add(user.Login.ToLower(), user);
			}
		}

		public UsersTable GetCurrentUser() {
			string login = HttpContext.Current.User.Identity.Name.ToLower() ;
			if (AllUsers.ContainsKey(login)) {
				return AllUsers[login];
			} else {
				UsersTable user = new UsersTable();
				user.Login = login;
				user.Name = "Вход не выполнен: " + login;
				return user;
			}
		}
	}
}