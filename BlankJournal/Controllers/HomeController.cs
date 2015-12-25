using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BlankJournal.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View("MainPage");
        }

				public ActionResult getFile(string id) {
					BlanksEntities eni = new BlanksEntities();
					IQueryable<DataTable> data = from d in eni.DataTable where d.ID == id select d;
					if (data.Count() > 0) {
						DataTable dt = data.First();
						string fn=dt.ID.ToString()+(dt.isPDF==true?".pdf":".docx");
						string fullpath=Server.MapPath("/TempData/")+fn;
						System.IO.File.WriteAllBytes(fullpath, dt.Data);
						Response.Redirect("/TempData/" + fn);													
						
					}
					return View("View1");
				}

    }
}
