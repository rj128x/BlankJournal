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

		public User() {
			Login = "noname";
			Name = "noname";
			CanEditTBP = false;
			CanDoOper = false;
		}

		public User(UsersTable tbl) {
			Login = tbl.Login;
			Name = tbl.Name;
			CanDoOper = tbl.CanDoOper;
			CanEditTBP = tbl.CanEditTBP;
		}
	}
}