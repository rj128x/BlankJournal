using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BlankJournal.Models {
	public class InitDB {
		public static void doInit(DirectoryInfo dir) {
			DirectoryInfo[] dirs=dir.GetDirectories();
			foreach (DirectoryInfo d in dirs) {
				doInit(d);
			}
			DateTime load = DateTime.Now;
			FileInfo[] files = dir.GetFiles();
			BlanksEntities eni = new BlanksEntities();
			foreach (FileInfo file in files) {
				Logger.info("Обработка файла " + file.Name.ToString());
				FileStream str = file.OpenRead();
				byte[] buffer = new byte[str.Length];
				str.Read(buffer, 0, (int)str.Length);
				str.Close();
				processFileData(file.Name, buffer,load);
				
			}
			
		}

		public static ReturnMessage processFileData(string fileName, byte[] data,DateTime dateLoad) {
			Logger.info("Обработка файла " + fileName.ToString());
			BlanksEntities eni = new BlanksEntities();
			int iEx = fileName.LastIndexOf(".");
			string ext = fileName.Substring(iEx).ToLower();
			string nameWithoutEx = fileName.Substring(0, iEx);
			dateLoad = new DateTime(dateLoad.Year, dateLoad.Month, dateLoad.Day, dateLoad.Hour, dateLoad.Minute, dateLoad.Second);
			if (ext.Contains("pdf") || ext.Contains("docx")) {
				try {
					string num = "";
					try {
						int i = fileName.IndexOf(" ");
						if (i > 0)
							num = fileName.Substring(0, i);
						else {
							num = nameWithoutEx;
						}
					} catch {
					}
					if (num.Length == 0) {
						Logger.info("===Бланк не распознан");
						return new ReturnMessage(false, String.Format("{0} - Бланк не распознан", fileName));
					}
					var list = (from t in eni.TBPInfoTable
								  from dat in eni.DataTable.Where(dat => dat.ID == t.DataPDF).DefaultIfEmpty()
								  from dat2 in eni.DataTable.Where(dat2 => dat2.ID == t.DataWord).DefaultIfEmpty()
								  where t.Number == num
								  select new { blank = t, md5PDF = dat.md5, md5Word = dat2.md5 }).FirstOrDefault();
					if (list!=null) {
						Logger.info("Бланк найден в БД: " + num);
						string md5 = MD5Class.getString(data);
						if (md5 == list.md5Word || md5 == list.md5PDF) {
							return new ReturnMessage(true, string.Format("{0} Файл не изменен", fileName));
						}

						TBPInfoTable tbp = list.blank;


						TBPHistoryTable hist = (from h in eni.TBPHistoryTable where h.TBPNumber == tbp.Number && h.DateCreate==dateLoad
																			 select h).FirstOrDefault();
						if (hist == null) {
							hist = new TBPHistoryTable();
							hist.DateCreate = dateLoad;
							hist.Author = DBContext.Single.GetCurrentUser().Login;
							hist.Id = Guid.NewGuid().ToString();
							hist.TBPNumber = tbp.Number;
							hist.TBPID = tbp.ID;
							eni.TBPHistoryTable.Add(hist);
						}

						DataTable dat = new DataTable();
						dat.Author = DBContext.Single.GetCurrentUser().Login;
						dat.DateCreate = DateTime.Now;
						dat.ID = Guid.NewGuid().ToString();
						dat.Data = data;
						dat.isPDF = ext.Contains("pdf");
						dat.FileInfo = fileName;
						dat.md5 = md5;

						if (dat.isPDF) {
							hist.PrevPDFData = tbp.DataPDF;
							tbp.DataPDF = dat.ID;
							hist.NewPDFData = dat.ID;
						} else {
							hist.PrevWordData = tbp.DataWord;
							tbp.DataWord = dat.ID;
							hist.NewWordData = dat.ID;
						}
						eni.DataTable.Add(dat);

						eni.SaveChanges();
						return new ReturnMessage(true, string.Format("{0} сохранен в бланк {1}", fileName, tbp.Number));
					} else {
						return new ReturnMessage(true, string.Format("{0} не найден в БД", fileName));
					}
				} catch (Exception e) {
					Logger.info("Ошибка при добавлении бланка в БД" + e.ToString());
					return new ReturnMessage(false, String.Format("{0} - Ошибка при добавлении бланка в БД", fileName));
				}
			} else {
				return new ReturnMessage(false, String.Format("{0} - Не файл бланка", fileName));
			}
		}
	}
}