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
using System.Windows.Navigation;
using MainSL.MainSVC;

namespace MainSL.Views {
	public partial class JournalPage : Page {
		public static bool inited { get; set; }
		public JournalPage() {
			InitializeComponent();
			if (!inited) {
				GlobalContext.Single.Client.GetJournalBPCompleted += Client_GetJournalBPCompleted;
				GlobalContext.Single.Client.CreateBPCompleted += Client_CreateBPCompleted;
				GlobalContext.Single.Client.GetJournalBPAsync();
			}
			inited = true;
		}

		void Client_CreateBPCompleted(object sender, MainSVC.CreateBPCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			ReturnMessage msg = e.Result as ReturnMessage;
			MessageBox.Show(msg.Message);
		}

		void Client_GetJournalBPCompleted(object sender, MainSVC.GetJournalBPCompletedEventArgs e) {
			grdBlanks.ItemsSource = e.Result;
		}

		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {
		}

		private void btnShow_Click(object sender, RoutedEventArgs e) {
			JournalRecord blank = grdBlanks.SelectedItem as JournalRecord;
			FloatWindow.OpenWindow("Home/getFile?id=" + blank.IDWordData);
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			JournalRecord newBlank = grdBlanks.SelectedItem as JournalRecord;
			newBlank.isInit = false;
			JournalRecordWindow win = new JournalRecordWindow();
			win.Init(newBlank);
			win.Closed += win_Closed;
			win.Show();
		}

		void win_Closed(object sender, EventArgs e) {
			JournalRecordWindow win = sender as JournalRecordWindow;
			if (win.DialogResult == true) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.CreateBPAsync(win.CurrentBlank);
			}
		}

	}
}
