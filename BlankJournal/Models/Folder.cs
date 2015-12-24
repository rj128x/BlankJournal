using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BlankJournal.Models {
	public class Folder {
		public int ID { get; set; }
		public string Name { get; set; }

		public Folder() {

		}

		public Folder(FoldersTable tbl) {
			ID = tbl.Id;
			Name = tbl.Name;
		}
	}
}