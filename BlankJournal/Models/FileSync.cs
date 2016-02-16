using ConnectUNCWithCredentials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Tools;

namespace BlankJournal.Models {


	public class FileSync {
		//public static string Folder = @"\\sr-votges-013.corp.gidroogk.com\Рабочие_документы$\Предприятие\Оперативная служба\Группа режимов\ВЭР\AutoArchive";
		public static bool SyncTBP(TBPInfoTable tbl) {
			try {
				Logger.info("Синхронизация бланка " + tbl.Number + " " + tbl.Name);
				BlanksEntities eni = new BlanksEntities();
				FoldersTable folder = (from f in eni.FoldersTable where f.Id == tbl.Folder select f).FirstOrDefault();
				string path = string.Format(@"{0}\{1}", Settings.Single.archiveFolder, folder.Id + " " + folder.Name);

				bool ok = true;
				string shortname = tbl.Name.Length < 100 ? tbl.Name : tbl.Name.Substring(0, 100);
				if (!string.IsNullOrEmpty(tbl.DataPDF)) {
					string name = tbl.Number + " " + shortname + ".pdf";
					ok = ok && SyncFile(tbl.DataPDF, path, name, tbl.Number + " ");
				}
				if (!string.IsNullOrEmpty(tbl.DataWord)) {
					string name = tbl.Number + " " + shortname + ".docx";
					ok = ok && SyncFile(tbl.DataWord, path, name, tbl.Number + " ");
				}
				return ok;
			}
			catch (Exception e) {
				Logger.info("Ошибка при синхронизации ТБП " + tbl.Number + " " + e.ToString());
				return false;
			}
		}

		public static bool SyncFile(string id, string path, string fileName, string number) {
			Logger.info("Запись файла " + id);
			try {
				using (Impersonator imp = new Impersonator(Settings.Single.syncUser, Settings.Single.syncDomain, Settings.Single.syncPassword)) {

					BlanksEntities eni = new BlanksEntities();
					DataTable tbl = (from t in eni.DataTable where t.ID == id select t).FirstOrDefault();
					if (tbl == null) {
						Logger.info("Файл не найден " + id);
					}
					path = path + (tbl.isPDF ? @"\PDF" : "");
					if (!Directory.Exists(path))
						Directory.CreateDirectory(path);
					DirectoryInfo di = new DirectoryInfo(path);
					FileInfo[] files = di.GetFiles();
					foreach (FileInfo fi in files) {
						if (fi.Name.IndexOf(number) == 0) {
							fi.Delete();
						}
					}
					System.IO.File.WriteAllBytes(path + "\\" + fileName, tbl.Data);
					return true;
				}
			}
			catch (Exception e) {
				Logger.info("Ошибка при записи файла " + id);
				Logger.info(e.ToString());
				return false;
			}

		}



	}
}