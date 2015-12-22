using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal {
	public class Settings {
		public String DBServer { get; set; }
		public String DBUser { get; set; }
		public string DBPassword { get; set; }
		public String DBName { get; set; }
		public static Settings Single { get; protected set; }
		public static void init(string filename) {
			try {
				Settings single = XMLSer<Settings>.fromXML(filename);
				Single = single;
			} catch (Exception e) {
				Logger.error("Ошибка при чтении файла настроек " + e, Logger.LoggerSource.server);
			}
		}
	}
}