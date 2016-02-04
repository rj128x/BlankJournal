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
using System.ComponentModel;

namespace MainSL.Views {
	public partial class JournalPage : Page , INotifyPropertyChanged
	{		
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		private JournalAnswer _currentFilter;

		protected JournalAnswer CurrentFilter {
			get { return _currentFilter; }
			set { _currentFilter = value;
			NotifyChanged("CurrentFilter");
			}
		}

		public JournalPage() {
			InitializeComponent();
			init();
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.GetJournalBPAsync(CurrentFilter);
			cntrlJournal.OnEditButtonPressed += cntrlJournal_OnEditButtonPressed;
			cntrlJournal.OnDelButtonPressed += cntrlJournal_OnDelButtonPressed;
		}

		

		void cntrlJournal_OnEditButtonPressed(JournalRecord blank) {
			GlobalContext.Single.IsBusy = false;
			blank.isInit = false;
			JournalRecordWindow win = new JournalRecordWindow();
			win.Init(blank);
			win.Closed += win_Closed;
			win.Show();
		}

		public void init() {
			GlobalContext.Single.Client.GetJournalBPCompleted += Client_GetJournalBPCompleted;
			GlobalContext.Single.Client.removeBPCompleted += Client_removeBPCompleted;
		}

		

		public void deInit() {
			GlobalContext.Single.Client.GetJournalBPCompleted -= Client_GetJournalBPCompleted;
			GlobalContext.Single.Client.removeBPCompleted -= Client_removeBPCompleted;
		}

		void Client_GetJournalBPCompleted(object sender, MainSVC.GetJournalBPCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			CurrentFilter = e.Result as JournalAnswer;
			pnlFilter.DataContext = CurrentFilter;
			cntrlJournal.grdBlanks.ItemsSource = CurrentFilter.Data;
		}

		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e) {
			deInit();
			base.OnNavigatedFrom(e);
		}

		void win_Closed(object sender, EventArgs e) {
			JournalRecordWindow win = sender as JournalRecordWindow;
			//if (win.DialogResult == true) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.GetJournalBPAsync(CurrentFilter);
			//}
		}

		private void btnRefresh_Click(object sender, RoutedEventArgs e) {
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.GetJournalBPAsync(CurrentFilter);
		}

		private void btnPrint_Click(object sender, RoutedEventArgs e) {			
			FloatWindow.OpenWindow(String.Format("/Print/PrintJournalBP?year1={0}&month1={1}&day1={2}&year2={3}&month2={4}&day2={5}",
				CurrentFilter.dateStart.Value.Year,CurrentFilter.dateStart.Value.Month,CurrentFilter.dateStart.Value.Day,
				CurrentFilter.dateEnd.Value.Year,CurrentFilter.dateEnd.Value.Month,CurrentFilter.dateEnd.Value.Day));
		}

		void Client_removeBPCompleted(object sender, removeBPCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			MessageBox.Show(e.Result.Message);
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.GetJournalBPAsync(CurrentFilter);
		}

		void cntrlJournal_OnDelButtonPressed(JournalRecord blank) {
			if (MessageBox.Show("Вы уверены что хотите удалить запись в журнале " + blank.ShortNumber + "?", "Удаление!", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.removeBPAsync(blank);
			}
		}

	}
}
