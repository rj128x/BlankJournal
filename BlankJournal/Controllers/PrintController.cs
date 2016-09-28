using BlankJournal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlankJournal.Controllers
{
    public class PrintController : Controller
    {
        //
        // GET: /Print/

			public ActionResult PrintJournalBP(int year1,int month1, int day1, int year2, int month2,int day2) {
				JournalAnswer filter=new JournalAnswer();
				filter.dateStart=new DateTime(year1,month1,day1);
				filter.dateEnd = new DateTime(year2, month2, day2);
				filter.tbpID = -1;
				JournalAnswer result = DBContext.Single.GetJournalBP(filter);
				ViewResult view = View("PrintJournalBP", result);
				return view;
			}

    }
}
