using BlankJournal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace BlankJournal {
	// Примечание: Инструкции по включению классического режима IIS6 или IIS7 
	// см. по ссылке http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : System.Web.HttpApplication {
		protected void Application_Start() {
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			
			Logger.init(Server.MapPath("/logs/"), "blanks");
			Logger.info("Start",Logger.LoggerSource.server);

			Settings.init(Server.MapPath("/bin/Data/MainSettings.xml"));
			DBContext.Init();
			//BlanksModel.init();
		}
	}
}