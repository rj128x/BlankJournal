using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BlankJournal.Models
{
	public class Folder
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public int Ident { get; set; }
		public bool CanEdit { get; set; }

		public Folder() {

		}

		public Folder(FoldersTable tbl) {
			ID = tbl.Id;
			Name = tbl.Name;
			Ident = tbl.Ident;
			CanEdit = tbl.Id != "-" && tbl.Id != "del";
		}

		public static ReturnMessage editFolder(Folder newFolder, bool edit) {
			Logger.info("Редактирование папки " + newFolder.ID);
			try {
				string Message = "";
				BlanksEntities eni = new BlanksEntities();
				FoldersTable fld = (from f in eni.FoldersTable where f.Id == newFolder.ID && f.Ident != newFolder.Ident select f).FirstOrDefault();
				if (fld != null) {
					return new ReturnMessage(false, String.Format("Папка с кодом {0} уже существует. ", newFolder.ID));
				}
				FoldersTable tbl = new FoldersTable();
				bool ok = false;
				if (!edit) {
					try {
						Logger.info("получение индекса папки");
						int index = (from f in eni.FoldersTable orderby f.Ident descending select f.Ident).FirstOrDefault();
						tbl.Ident = index + 1;
					} catch (Exception e) {
						tbl.Ident = 1;
					}
					Logger.info("Присвоен индекс " + tbl.Ident);
					tbl.Id = newFolder.ID;
					tbl.Name = newFolder.Name;
					eni.FoldersTable.Add(tbl);
				}
				if (edit) {
					try {
						tbl = (from f in eni.FoldersTable where f.Ident == newFolder.Ident select f).FirstOrDefault();
						if (tbl == null)
							return new ReturnMessage(false, "Папка не найдена " + tbl.Id);
						tbl.Name = newFolder.Name;
						if (tbl.Id != newFolder.ID) {
							try {
								eni.FoldersTable.Remove(tbl);
								Logger.info("Изменение кода папки. Изменение привязки всех бланков. ");
								Message = "Изменена привязка бланков. Проверьте имена бланков \r\n";
								IQueryable<TBPInfoTable> tbpList = (from t in eni.TBPInfoTable where t.Folder == tbl.Id select t);
								foreach (TBPInfoTable tbp in tbpList) {
									tbp.Folder = "-";
								}
								FoldersTable tab = new FoldersTable();
								tab.Id = newFolder.ID;
								tab.Name = newFolder.Name;
								tab.Ident = newFolder.Ident;
								eni.FoldersTable.Add(tab);
								foreach (TBPInfoTable tbp in tbpList) {
									tbp.Folder = tab.Id;
								}

							} catch (Exception e) {
								Logger.info("Ошибка при перепревязке бланков " + e.ToString());
								return new ReturnMessage(false, "Ошибка при изменнии кода папки ");
							}
						}
					} catch (Exception e) {
						Logger.info("Ошибка при поиске папки для редактирования " + e.ToString());
						return new ReturnMessage(false, "Ошибка при редактировании папки");
					}
				}

				eni.SaveChanges();

				DBContext.Init();
				return new ReturnMessage(true, Message + "Изменения сохранены");
			} catch (Exception e) {
				Logger.info("Ошибка при редактирвоании папки " + e.ToString());
				return new ReturnMessage(false, "Ошибка при редактировании папки");
			}
		}

		public static ReturnMessage removeFolder(Folder folder) {
			Logger.info("Удаление папки " + folder.ID);
			try {
				BlanksEntities eni = new BlanksEntities();
				FoldersTable tbl = (from f in eni.FoldersTable where f.Ident == folder.Ident select f).FirstOrDefault();
				if (tbl.Id == "-" || tbl.Id == "del")
					return new ReturnMessage(false,"Ошибка в индексации папок");
				IQueryable<TBPInfoTable> tbpList = (from t in eni.TBPInfoTable where t.Folder == tbl.Id select t);
				foreach (TBPInfoTable tbp in tbpList) {
					tbp.Folder = "del";
					tbp.isActive = false;
				}
				eni.FoldersTable.Remove(tbl);
				eni.SaveChanges();
				DBContext.Init();
				return new ReturnMessage(true, String.Format("Удалена папка {0} \r\n Все бланки из этой папки перенсены в папку 'Удаленные'",folder.ID));
			} catch (Exception e) {
				Logger.info("Ошибка при удалении папки " + e.ToString());
				return new ReturnMessage(false, "Ошибка при удалении папки");
			}
		}
	}
}