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
				string shortname = "";
				if (tbl.Name.Contains('(') && tbl.Name.Length>90) {
					int ind = tbl.Name.IndexOf("(");
					shortname = tbl.Name.Substring(0, ind);
				} else {					
					shortname = tbl.Name.Length>90?tbl.Name.Substring(0,90):tbl.Name;	
				}			

				if (!string.IsNullOrEmpty(tbl.DataPDF)) {
					string name = tbl.Number + " " + shortname + ".pdf";
					ok = ok && SyncFile(tbl.DataPDF, path, name, tbl.Number + " ");
				}
				if (!string.IsNullOrEmpty(tbl.DataWord)) {
					string name = tbl.Number + " " + shortname + ".docx";
					ok = ok && SyncFile(tbl.DataWord, path, name, tbl.Number + " ");
					
					try {
						Logger.info("генерация ОБП");
						TBPInfo tbp=new TBPInfo(tbl);
						using (Impersonator imp = new Impersonator(Settings.Single.syncUser, Settings.Single.syncDomain, Settings.Single.syncPassword)) {
							string obpPath=path + @"\ОБП\";
							if (!Directory.Exists(obpPath))
								Directory.CreateDirectory(obpPath);

							DirectoryInfo di = new DirectoryInfo(obpPath);
							FileInfo[] files = di.GetFiles();
							string number = "ОБП " + tbl.Number + " ";
							foreach (FileInfo fi in files) {
								if (fi.Name.IndexOf(number) == 0) {
									try {
										Logger.info("Удаление файла " + fi.Name);
										fi.Delete();
									} catch (Exception e) {
										Logger.info("Ошибка при удалении файла " + e.ToString());
									}
								}
							}

							string file = WordData.createOBP(obpPath, tbp);
						}
					}
					catch (Exception e) {
						Logger.info("Ошибка при создании ОБП");
					}

				}
				return ok;
			}
			catch (Exception e) {
				Logger.info("Ошибка при синхронизации ТБП " + tbl.Number + " " + e.ToString());
				return false;
			}
		}

		public static bool SyncFile(string id, string path, string fileName, string number) {
			Logger.info("Запись файла " + id +" в файл "+fileName);
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
							try {
								Logger.info("Удаление файла " + fi.Name);
								fi.Delete();
							}catch (Exception e) {
								Logger.info("Ошибка при удалении файла " + e.ToString());
							}
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