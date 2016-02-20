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
	public delegate void EditBlankPressed(JournalRecord blank);
	public partial class JournalGridControl : UserControl {
		public event EditBlankPressed OnEditButtonPressed;
		public event EditBlankPressed OnDelButtonPressed;
		public event EditBlankPressed OnUnblockButtonPressed;

		public JournalGridControl() {
			InitializeComponent();
		}


		private void btnShow_Click(object sender, RoutedEventArgs e) {
			JournalRecord blank = grdBlanks.SelectedItem as JournalRecord;

			FloatWindow.OpenWindow("Home/getBlank?id=" + blank.Number);
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e) {
			JournalRecord newBlank = grdBlanks.SelectedItem as JournalRecord;
			if (OnEditButtonPressed != null)
				OnEditButtonPressed(newBlank);
		}

		private void grdBlanks_LoadingRow(object sender, DataGridRowEventArgs e) {
			e.Row.Header = e.Row.GetIndex() + 1;
		}

		private void btnDelete_Click(object sender, RoutedEventArgs e) {
			JournalRecord newBlank = grdBlanks.SelectedItem as JournalRecord;
			if (OnDelButtonPressed != null)
				OnDelButtonPressed(newBlank);
		}

		private void btnUnblock_Click(object sender, RoutedEventArgs e) {
			JournalRecord newBlank = grdBlanks.SelectedItem as JournalRecord;
			if (OnUnblockButtonPressed != null)
				OnUnblockButtonPressed(newBlank);
		}

		private void grdBlanks_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			try {
				JournalRecord current = grdBlanks.SelectedItem as JournalRecord;
				current.CanUnblock = current.Closed && GlobalContext.Single.CurrentUser.CanDoOper && GlobalContext.Single.CurrentUser.CanEditUsers;
				foreach (JournalRecord rec in grdBlanks.ItemsSource) {
					if (rec != current)
						rec.CanUnblock = false;
				}
			}
			catch { }
		}

	}
}
