using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal {
	public class Settings {
		public String smtpServer { get; set; }
		public String smtpUser { get; set; }
		public string smtpPassword { get; set; }
		public string smtpDomain { get; set; }
		public string smtpFrom { get; set; }
		public int smtpPort { get; set; }
		public string archiveFolder { get; set; }
		public string syncUser { get; set; }
		public string syncPassword { get; set; }
		public string syncDomain { get; set; }


		public static Settings Single { get; protected set; }
		public static void init(string filename) {
			try {
				Settings single = XMLSer<Settings>.fromXML(filename);
				Single = single;
				Logger.info(single.archiveFolder);
			} catch (Exception e) {
				Logger.error("Ошибка при чтении файла настроек " + e, Logger.LoggerSource.server);
			}
		}
	}
}