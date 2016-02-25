using MainSL.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainSL {
	public partial class MainPage : UserControl {
		public MainPage() {
			InitializeComponent();
		}

		public void init() {
			GlobalContext.init();
			if (Application.Current.IsRunningOutOfBrowser) {
				// Проверка наличия новых версий
				GlobalContext.Single.IsLocked = true;
				GlobalContext.Single.LockedText = "Проверка новых версий";
				clearTempFiles();
				Application.Current.CheckAndDownloadUpdateCompleted += Current_CheckAndDownloadUpdateCompleted;
				Application.Current.CheckAndDownloadUpdateAsync();
			}
			else {
				startLoad();
				InstallWindow win = new InstallWindow();
				win.Installed = Application.Current.InstallState == System.Windows.InstallState.Installed;
				win.Show();
			}
		}

		private void Current_CheckAndDownloadUpdateCompleted(object sender,
		CheckAndDownloadUpdateCompletedEventArgs e) {
			if (e.UpdateAvailable) {
				GlobalContext.Single.LockedText = "Установлена новая версия. Перезапустите приложение.";
				GlobalContext.Log("Установка обновлений");
				GlobalContext.Single.IsLocked = true;
				// Здесь можно ввести код вызова пользовательского 
				// метода в объекте MainPage, который отключает интерфейс
			}
			else if (e.Error != null && e.Error is PlatformNotSupportedException) {
				GlobalContext.Single.LockedText = "Есть новые версии приложения"
					 + "однако для их применения необходима новая версия Silverlight. " +
					 "Посетите сайт http://silverlight.net для обновления Silverlight.";
			}
			else {
				GlobalContext.Single.IsLocked = false;
				GlobalContext.Log("Нет обновлений");
				startLoad();
			}
		}

		void Single_onFinishLoad() {
			GlobalContext.Log("Загрузка завершена");
			this.ContentFrame.Source = new Uri("/Home", UriKind.Relative);
		}

		public void startLoad() {
			GlobalContext.Log("Загрузка пользоветелей и папок");
			GlobalContext.Single.onFinishLoad += Single_onFinishLoad;
			GlobalContext.Single.Connect();
		}

		public void clearTempFiles() {
			try {
				DirectoryInfo dir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TempTBP");
				IEnumerable<FileInfo> files = dir.EnumerateFiles();
				foreach (FileInfo file in files) {
					if (file.Extension.ToLower().Contains("docx") && file.CreationTime.AddDays(10) < DateTime.Now) {
						try {
							file.Delete();
						}
						catch { }
					}
				}
			}
			catch (Exception e) {

			}
		}

		// После перехода в фрейме убедиться, что выбрана кнопка HyperlinkButton, представляющая текущую страницу
		private void ContentFrame_Navigated(object sender, NavigationEventArgs e) {
			foreach (UIElement child in LinksStackPanel.Children) {
				HyperlinkButton hb = child as HyperlinkButton;
				if (hb != null && hb.NavigateUri != null) {
					if (hb.NavigateUri.ToString().Equals(e.Uri.ToString())) {
						VisualStateManager.GoToState(hb, "ActiveLink", true);
					}
					else {
						VisualStateManager.GoToState(hb, "InactiveLink", true);
					}
				}
			}
		}

		// Если во время навигации возникает ошибка, отобразить окно ошибки
		private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e) {
			e.Handled = true;
			ChildWindow errorWin = new ErrorWindow(e.Uri);
			errorWin.Show();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e) {
			init();
		}
	}
}