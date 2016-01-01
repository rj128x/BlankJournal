﻿using System;
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
		public JournalPage() {
			InitializeComponent();
			init();
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.GetJournalBPAsync();
		}

		public void init() {
			GlobalContext.Single.Client.GetJournalBPCompleted += Client_GetJournalBPCompleted;
		}

		public void deInit() {
			GlobalContext.Single.Client.GetJournalBPCompleted -= Client_GetJournalBPCompleted;
		}

		void Client_GetJournalBPCompleted(object sender, MainSVC.GetJournalBPCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			grdBlanks.ItemsSource = e.Result;
		}

		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e) {
			deInit();
			base.OnNavigatedFrom(e);
		}

		private void btnShow_Click(object sender, RoutedEventArgs e) {
			JournalRecord blank = grdBlanks.SelectedItem as JournalRecord;

			FloatWindow.OpenWindow("Home/getBlank?id=" + blank.Number);
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
			//if (win.DialogResult == true) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.GetJournalBPAsync();
			//}
		}

	}
}
