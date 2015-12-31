using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BlankJournal.Models {
	public class User {
		public string Login { get; set; }
		public string Name { get; set; }
		public bool CanEditTBP { get; set; }
		public bool CanDoOper {get;set;}
		public bool CanCommentTBP { get; set; }
		public bool IsEditing { get; set; }

		public User() {
			Login = "noname";
			Name = "noname";
			CanEditTBP = false;
			CanDoOper = false;
			CanCommentTBP = false;
		}

		public User(UsersTable tbl) {
			Login = tbl.Login;
			Name = tbl.Name;
			CanDoOper = tbl.CanDoOper;
			CanEditTBP = tbl.CanEditTBP;
			CanCommentTBP = tbl.CanCommentTBP;
		}

		public static ReturnMessage EditUser(User user) {
			Logger.info("Добавление/редактирование пльзователя");
			try {
				BlanksEntities eni=new BlanksEntities();
				UsersTable last = (from u in eni.UsersTable where u.Login.ToLower() == user.Login.ToLower() select u).FirstOrDefault();
				if (last == null) {
					last = new UsersTable();
					last.Login = user.Login;
					eni.UsersTable.Add(last);
				}
				last.Name = user.Name;
				last.CanEditTBP = user.CanEditTBP;
				last.CanDoOper = user.CanDoOper;
				last.CanCommentTBP=user.CanCommentTBP;
				eni.SaveChanges();
				DBContext.Single.InitUsers();
				return new ReturnMessage(true, "Информация о пользоватле сохранена");
			} catch (Exception e) {
				Logger.info("Ошибка при добавлении/изменении пользователя"+e.ToString());
				return new ReturnMessage(false, "Ошибка при добавлении/изменении пользователя");
			}
		}
	}
}