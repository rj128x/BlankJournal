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
		public bool SendMailComment { get; set; }
		public bool CanEditUsers { get; set; }
		public bool IsEditing { get; set; }
		public string Mail { get; set; }
		public string AvailEditFolders { get; set; }
		public List<string> AvailFoldersList { get; set; }
		public bool CanEditTBPCurrentFolder { get; set; }
		public bool IsAdmin { get; set; }
		public bool ShowRemovedTBP { get; set; }
		

		public User() {
			Login = "noname";
			Name = "noname";
			CanEditTBP = false;
			CanDoOper = false;
			CanCommentTBP = false;
			AvailFoldersList = new List<string>();
		}

		public User(UsersTable tbl) {
			Login = tbl.Login;
			Name = tbl.Name;
			CanDoOper = tbl.CanDoOper;
			CanEditTBP = tbl.CanEditTBP;
			CanCommentTBP = tbl.CanCommentTBP;
			SendMailComment = tbl.SendMailComments;
			CanEditUsers = tbl.CanEditUsers;
			AvailEditFolders = tbl.AvailEditFolders;
			AvailFoldersList = new List<string>();
			IsAdmin = tbl.Admin;

			if (AvailEditFolders == "0") {
				try {
					foreach (Folder folder in DBContext.Single.AllFolders.Values) {
						AvailFoldersList.Add(folder.ID);
					}
				}catch (Exception e) {
					Logger.info("Ошибка при формировании списка доступных папок "+e.ToString());
				}
			} else {
				try {
					string[] folders = AvailEditFolders.Split(';');
					foreach (string folder in folders) {
						AvailFoldersList.Add(folder);
					}
				} catch (Exception e) {
					Logger.info("Ошибка при формировании списка доступных папок "+e.ToString());
				}
			}
			Mail = tbl.Mail;
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
				last.Mail = user.Mail;
				last.SendMailComments = user.SendMailComment;
				last.CanEditUsers = user.CanEditUsers;
				last.AvailEditFolders = user.AvailEditFolders;
				last.Admin = user.IsAdmin;
				
				eni.SaveChanges();
				DBContext.Single.InitUsers();
				return new ReturnMessage(true, "Информация о пользоватле сохранена");
			} catch (Exception e) {
				Logger.info("Ошибка при добавлении/изменении пользователя"+e.ToString());
				return new ReturnMessage(false, "Ошибка при добавлении/изменении пользователя");
			}
		}

		public static ReturnMessage DeleteUser(User user) {
			Logger.info("удаление пльзователя");
			try {
				BlanksEntities eni = new BlanksEntities();
				UsersTable last = (from u in eni.UsersTable where u.Login.ToLower() == user.Login.ToLower() select u).FirstOrDefault();
				if (last == null) {
					return new ReturnMessage(false,"Пользователь не найден");
				}
				eni.UsersTable.Remove(last);
				eni.SaveChanges();
				DBContext.Single.InitUsers();
				return new ReturnMessage(true, "Пользователь удален из системы");
			} catch (Exception e) {
				Logger.info("Ошибка при удалении пользователя" + e.ToString());
				return new ReturnMessage(false, "Ошибка при удалении пользователя");
			}
		}
	}
}