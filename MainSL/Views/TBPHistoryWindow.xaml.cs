using MainSL.MainSVC;
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

namespace MainSL.Views {
	public partial class TBPHistoryWindow : ChildWindow {
		public TBPHistoryWindow() {
			InitializeComponent();
		}

		public TBPInfo CurrentTBP;

		public void init(TBPInfo tbp) {
			CurrentTBP = tbp;
			GlobalContext.Single.Client.getHistoryCompleted += Client_getHistoryCompleted;
			GlobalContext.Single.Client.removeHistoryRecordCompleted += Client_removeHistoryRecordCompleted;
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.getHistoryAsync(tbp);
		}

		public void deInit() {
			GlobalContext.Single.Client.getHistoryCompleted -= Client_getHistoryCompleted;
			GlobalContext.Single.Client.removeHistoryRecordCompleted -= Client_removeHistoryRecordCompleted;
		}



		void Client_getHistoryCompleted(object sender, getHistoryCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			grdTBPHistory.ItemsSource = e.Result;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}

		private void btnPrevPDF_Click(object sender, RoutedEventArgs e) {
			TBPHistoryRecord rec = grdTBPHistory.SelectedItem as TBPHistoryRecord;
			FloatWindow.OpenWindow("/Home/getFile?id=" + rec.PrevPDFID);
		}

		private void btnPrevWord_Click(object sender, RoutedEventArgs e) {
			TBPHistoryRecord rec = grdTBPHistory.SelectedItem as TBPHistoryRecord;
			FloatWindow.OpenWindow("/Home/getFile?id=" + rec.PrevWordID);
		}

		private void btnNewPDF_Click(object sender, RoutedEventArgs e) {
			TBPHistoryRecord rec = grdTBPHistory.SelectedItem as TBPHistoryRecord;
			FloatWindow.OpenWindow("/Home/getFile?id=" + rec.NewPDFID);
		}

		private void btnNewWord_Click(object sender, RoutedEventArgs e) {
			TBPHistoryRecord rec = grdTBPHistory.SelectedItem as TBPHistoryRecord;
			FloatWindow.OpenWindow("/Home/getFile?id=" + rec.NewWordID);
		}

		private void ChildWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			deInit();
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e) {
			try {
				TBPHistoryRecord rec = grdTBPHistory.SelectedItem as TBPHistoryRecord;
				if (MessageBox.Show("Вы уверены что хотите удалить запись в истории редактирования?", "Удаление", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
					GlobalContext.Single.IsBusy = true;
					GlobalContext.Single.Client.removeHistoryRecordAsync(rec);
				}
			}
			catch { }
		}

		void Client_removeHistoryRecordCompleted(object sender, removeHistoryRecordCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			MessageBox.Show(e.Result.Message);
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.getHistoryAsync(CurrentTBP);
		}
	}
}

