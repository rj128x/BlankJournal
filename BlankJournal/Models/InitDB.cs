using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class InitDB {
		public static void doInit(DirectoryInfo dir) {
			DirectoryInfo[] dirs=dir.GetDirectories();
			foreach (DirectoryInfo d in dirs) {
				doInit(d);
			}
			FileInfo[] files = dir.GetFiles();
			BlanksEntities eni = new BlanksEntities();
			foreach (FileInfo file in files) {
				Logger.info("Обработка файла " + file.Name.ToString());
				if (file.Extension.ToLower().Contains("pdf") || file.Extension.ToLower().Contains("docx")) {
					try {
						string num = "";						
						try {
												
							int i = file.Name.IndexOf(" ");
							if (i > 0)
								num = file.Name.Substring(0, i);
							else {
								string ex = file.Extension;
								int posEx = file.Name.IndexOf(ex);
								num = file.Name.Substring(0, posEx);		
							}
						} catch {							
						}
						if (num.Length == 0) {
							Logger.info("===Бланк не распознан");
							continue;
						}
						IQueryable<TBPInfoTable> list = from t in eni.TBPInfoTable where t.Number == num select t;
						if (list.Count() == 1) {
							Logger.info("Бланк найден в БД: "+num);
							TBPInfoTable tbp = list.First();							
							FileStream str = file.OpenRead();
							byte[] buffer = new byte[str.Length];
							str.Read(buffer, 0, (int)str.Length);
							str.Close();

							DataTable dat = new DataTable();
							dat.Author = DBContext.Single.GetCurrentUser().Login;
							dat.DateCreate = DateTime.Now;
							dat.ID = Guid.NewGuid().ToString();
							dat.Data = buffer;
							dat.isPDF = file.Extension.ToLower().Contains("pdf");

							if (dat.isPDF) {
								tbp.DataPDF = dat.ID;
							} else {
								tbp.DataWord = dat.ID;
							}
							eni.DataTable.Add(dat);

							eni.SaveChanges();
						}
					} catch (Exception e) {
						Logger.info("Ошибка при добавлении бланка в БД" +e.ToString());
					}					
				}
			}
			
		}
	}
}