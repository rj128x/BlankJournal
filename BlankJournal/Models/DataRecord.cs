using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class DataRecord {
		public string ID { get; set; }
		public byte[] Data { get; set; }
		public string FileInfo { get; set; }
		public string md5 { get; set; }
		public DateTime DateCreate { get; set; }
		public string Author { get; set; }

		public DataRecord() {

		}

		public DataRecord(DataTable tbl) {
			ID = tbl.ID;
			Data = tbl.Data;
			FileInfo = tbl.FileInfo;
			md5 = tbl.md5;
			DateCreate = tbl.DateCreate;
		}

		public static DataRecord getDataRecord(string id) {
			Logger.info("Получение файла из БД " + id);
			try {
				BlanksEntities eni=new BlanksEntities();
				DataTable tbl = (from t in eni.DataTable where t.ID == id select t).FirstOrDefault();
				if (tbl != null) {
					return new DataRecord(tbl);
				} else {
					Logger.info("Файл не найден");
				}
			} catch (Exception e) {
				Logger.info("ошибка при получении файла " + e.ToString());
			}
			return null;
		}

		public static ReturnMessage UpdateFile(DataRecord rec) {
			Logger.info(String.Format("Изменение файла {0}({1}) " , rec.ID,rec.FileInfo));
			try {
				BlanksEntities eni = new BlanksEntities();
				DataTable tbl = (from t in eni.DataTable where t.ID == rec.ID select t).FirstOrDefault();
				if (tbl != null) {
					tbl.Data = rec.Data;					
					eni.SaveChanges();
					return new ReturnMessage(true,"Файл обновлен в БД");
				} else {
					return new ReturnMessage(false, "Файл не найден");
				}
			} catch (Exception e) {
				Logger.info(e.ToString());
				return new ReturnMessage(false, "Ошибка при обновлении файла в БД");
			}

		}
	}
}