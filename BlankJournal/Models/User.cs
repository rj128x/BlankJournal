using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BlankJournal.Models {
	public class User {
		public string Login { get; set; }
		public string Name { get; set; }
		public bool IsNSS { get; set; }
		public bool IsNOS{get;set;}

		public User() {
			Login = "noname";
			Name = "noname";
			IsNOS = false;
			IsNSS = false;
		}

		public User(UsersTable tbl) {
			Login = tbl.Login;
			Name = tbl.Name;
			IsNOS = tbl.IsNOS;
			IsNSS = tbl.IsNSS;
		}
	}
}