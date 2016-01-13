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

	}
}
