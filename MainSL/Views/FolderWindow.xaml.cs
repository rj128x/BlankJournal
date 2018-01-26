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
	public partial class FolderWindow : ChildWindow {
		public Folder CurrentFolder;
		public bool edit = false;
		public FolderWindow() {
			InitializeComponent();
		}


		private void OKButton_Click(object sender, RoutedEventArgs e) {
			if (string.IsNullOrEmpty(CurrentFolder.ID) || string.IsNullOrEmpty(CurrentFolder.Name)) {
				MessageBox.Show("Заполните поля!");
				return;
			}			
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.editFolderAsync(CurrentFolder,edit);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}

		public void init(Folder folder) {
			CurrentFolder = folder;
			pnlData.DataContext = CurrentFolder;
			GlobalContext.Single.Client.editFolderCompleted += Client_editFolderCompleted;
		}

		private void Client_editFolderCompleted(object sender, editFolderCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			ReturnMessage msg = e.Result as ReturnMessage;
			MessageBox.Show(msg.Message);
			if (msg.Result) {
				this.DialogResult = true;
			}
		}

		protected override void OnClosed(EventArgs e) {
			deinit();
			base.OnClosed(e);
		}

		public void deinit() {
			GlobalContext.Single.Client.editFolderCompleted -= Client_editFolderCompleted;

		}
	}
}

