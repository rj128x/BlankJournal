using BlankJournal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BlankJournal.Controllers {
	public class HomeController : Controller {
		//
		// GET: /Home/

		public ActionResult Index() {
			return View("MainPage");
		}

		public void processFile(string id) {
			try {
				Logger.info("Получение содержимого файла id=" + id);
				BlanksEntities eni = new BlanksEntities();
				DataTable data = (from d in eni.DataTable where d.ID == id select d).FirstOrDefault();
				if (data != null) {
					string fn = data.FileInfo.ToString();
					string fullpath = Server.MapPath("/TempData/") + fn;
					Logger.info("Формирование временного файла " + fullpath);
					System.IO.File.WriteAllBytes(fullpath, data.Data);

					Response.Redirect("/TempData/" + fn + "?" + Guid.NewGuid().ToString());
				} else {
					Logger.info("Файл не найден");
				}
			} catch (Exception e) {
				Logger.info("ошибка при получении одержимого файла" + e.ToString());
			}
		}

		public ActionResult getFile(string id) {
			processFile(id);
			return View("View1");
		}

		public ActionResult getBlank(string id) {
			try {
				Logger.info("Получение файла бланка из журнала переключений " + id);
				BlanksEntities eni = new BlanksEntities();
				BPJournalTable data = (from d in eni.BPJournalTable where d.Id == id select d).FirstOrDefault();
				if (data != null) {
					BPJournalTable dt = data;
					if (!dt.isOBP) {
						Logger.info("Переключения по ТБП. Формирование pdf  номером " + id);
						string fn = getPDFWithNumber(id);
						if (!string.IsNullOrEmpty(fn))
							Response.Redirect("/TempData/" + fn + "?" + Guid.NewGuid().ToString());
					} else {
						processFile(dt.WordData);
					}
				}
			} catch (Exception e) {
				Logger.info("Ошибка при получении файла бланка " + e.ToString());
			}
			return View("View1");
		}

		public ActionResult getOBPWord(string id) {
			Logger.info("Формирование файла ОБП для ТБП " + id);
			BlanksEntities eni = new BlanksEntities();
			TBPInfoTable data = (from d in eni.TBPInfoTable where d.Number == id && d.isActive select d).FirstOrDefault();
			if (data !=null) {
				TBPInfo tbp = new TBPInfo(data);
				string fn=WordData.createOBP(Server.MapPath("/TempData/"),tbp);
				Response.Redirect("/TempData/" + fn + "?" + Guid.NewGuid().ToString());
			}
			return View("View1");
		}

		public string getPDFWithNumber(string id) {
			Logger.info("Получение pdf с номером файла бланка из журнала переключений " + id);
			BlanksEntities eni = new BlanksEntities();
			var data = (from d in eni.BPJournalTable where d.Id == id
										  from dat in eni.DataTable.Where(dat => dat.ID == d.PDFData).DefaultIfEmpty()
										  select new { blank = d, data = dat }).FirstOrDefault();
			if (data!=null) {
				string fn = Server.MapPath("/TempData/"+data.data.FileInfo);
				int num = (int)(Math.Ceiling(data.blank.Number - data.blank.DateCreate.Year) * JournalRecord.MAX_BP_YEAR);
				PDFClass.addTBPNumber(data.data.Data, fn,data.blank.IDShort);
				return data.data.FileInfo;
			}
			return null;
		}

		public ActionResult SyncDB(string TBPIDS) {
			Logger.info("Синхронизация БД");
			List<string> result = new List<string>();
			BlanksEntities eni = new BlanksEntities();
			string[] tbps = TBPIDS.Split(new char[] {'~'});
			IQueryable<TBPInfoTable> blanks = from t in eni.TBPInfoTable where t.isActive && tbps.Contains(t.ID.ToString()) select t;
			foreach (TBPInfoTable tbp in blanks) {
				bool res = FileSync.SyncTBP(tbp);
				//bool res = true;
				string resStr = String.Format("Синхронизация бланка {0} ({1}): {2}", tbp.Number, tbp.Name, res);
				result.Add(resStr);
			}
			return View("SyncDB", result);
		}

		public ActionResult ClearHistory() {
			Logger.info("Очистка истории редактирования в БД");
			List<string> result = TBPHistoryRecord.ClearHystory();

			return View("SyncDB", result);
		}

		public ActionResult InitDB() {
			DirectoryInfo dir = new DirectoryInfo("d:/tbp/");
			BlankJournal.Models.InitDB.doInit(dir,DateTime.Now);
			return View("View1");
		}

	}
}
