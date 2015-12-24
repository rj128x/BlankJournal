using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MainSL {
	public partial class App : Application {
		public App() {
			this.Startup += this.Application_Startup;
			this.UnhandledException += this.Application_UnhandledException;			
			GlobalContext.init();
			GlobalContext.Single.onFinishLoad+=Single_onFinishLoad;
			InitializeComponent();
		}

		public void Single_onFinishLoad() {
			this.RootVisual = new MainPage();
		}

		private void Application_Startup(object sender, StartupEventArgs e) {			
			GlobalContext.Single.Connect();
		}

		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e) {
			// Если приложение выполняется вне отладчика, воспользуйтесь для сообщения об исключении
			// элементом управления ChildWindow.
			if (!System.Diagnostics.Debugger.IsAttached) {
				// ПРИМЕЧАНИЕ. Это позволит приложению выполняться после того, как исключение было выдано,
				// но не было обработано. 
				// Для рабочих приложений такую обработку ошибок следует заменить на код, 
				// оповещающий веб-сайт об ошибке и останавливающий работу приложения.
				e.Handled = true;
				ChildWindow errorWin = new ErrorWindow(e.ExceptionObject);
				errorWin.Show();
			}
		}
	}
}