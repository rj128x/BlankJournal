﻿using MainSL.Views;
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
			InitializeComponent();
		}

		public void Single_onFinishLoad() {
			
		}


		private void Application_Startup(object sender, StartupEventArgs e) {
			this.RootVisual = new MainPage();
			
			if (Application.Current.IsRunningOutOfBrowser) {
				// Проверка наличия новых версий
				Application.Current.CheckAndDownloadUpdateCompleted +=
					 Application_CheckAndDownloadUpdateComplete;
				Application.Current.CheckAndDownloadUpdateAsync();
			} else {
				(this.RootVisual as MainPage).startLoad();
				InstallWindow win = new InstallWindow();
				win.Installed = Application.Current.InstallState == System.Windows.InstallState.Installed;
				
				win.Show();
			}
		}

		private void Application_CheckAndDownloadUpdateComplete(object sender,
		CheckAndDownloadUpdateCompletedEventArgs e) {
			if (e.UpdateAvailable) {
				GlobalContext.Log("Установка обновлений");
				GlobalContext.Single.IsLocked = true;
				// Здесь можно ввести код вызова пользовательского 
				// метода в объекте MainPage, который отключает интерфейс
			} else if (e.Error != null && e.Error is PlatformNotSupportedException) {
				GlobalContext.Single.IsLocked = true;
				MessageBox.Show("Есть новые версии приложения"
					 + "однако для их применения необходима новая версия Silverlight. " +
					 "Посетите сайт http://silverlight.net для обновления Silverlight.");
			}
			else {
				GlobalContext.Log("Нет обновлений");
				(this.RootVisual as MainPage).startLoad();
			}
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