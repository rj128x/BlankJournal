using ConnectUNCWithCredentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {


	public class FileSync {
		public static string Folder = @"\\sr-votges-013.corp.gidroogk.com\Рабочие_документы$\Предприятие\Оперативная служба\Группа режимов\ВЭР\AutoArchive";
		public static void SyncFolders(TBPInfoTable tbl) {
			UNCAccessWithCredentials unc = new UNCAccessWithCredentials();
			unc.NetUseWithCredentials(Folder, "chekunovamv", "corp", "rJ320204");
			try {
				
			}
			catch (Exception e) {
				Logger.info("Ошибка при синхронизации ТБП "+tbl.Number+" " + e.ToString());
			}
		}



	}
}