using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal {
	public class BlanksModel {
		public static BlanksModel Single { get; protected set; }

		public Dictionary<string, User> Users;
		public static void init() {
			Single = new BlanksModel();
			Single.Users = DBClass.getAllUsers();
		}

	}
}