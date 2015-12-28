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
			BlanksEntities eni = new BlanksEntities();
			IQueryable<DataTable> data = from d in eni.DataTable where d.ID == id select d;
			if (data.Count() > 0) {
				DataTable dt = data.First();
				string fn = dt.ID.ToString() + (dt.isPDF == true ? ".pdf" : ".docx");
				string fullpath = Server.MapPath("/TempData/") + fn;
				
				System.IO.File.WriteAllBytes(fullpath, dt.Data);

				Response.Redirect("/TempData/" + fn);
			}
		}

		public ActionResult getFile(string id) {
			processFile(id);
			return View("View1");
		}

		public ActionResult getBlank(string id) {
			BlanksEntities eni = new BlanksEntities();
			IQueryable<BPJournalTable> data = from d in eni.BPJournalTable where d.Id == id select d;
			if (data.Count() > 0) {
				BPJournalTable dt = data.First();
				string fileID = dt.isOBP ? dt.WordData : dt.PDFData;
				processFile(fileID);
			}
			return View("View1");
		}

		public ActionResult getOBPWord(string id) {
			BlanksEntities eni = new BlanksEntities();
			IQueryable<TBPInfoTable> data = from d in eni.TBPInfoTable where d.Number == id select d;
			if (data.Count() > 0) {
				TBPInfo tbp = new TBPInfo(data.First());
				string fn=WordData.createOBP(Server.MapPath("/TempData/"),tbp);
				Response.Redirect("/TempData/" + fn);
			}
			return View("View1");
		}

		public ActionResult InitDB() {
			DirectoryInfo dir = new DirectoryInfo("D:/TBP");
			BlankJournal.Models.InitDB.doInit(dir);
			return View("View1");
		}

	}
}
