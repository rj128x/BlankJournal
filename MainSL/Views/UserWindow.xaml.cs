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
	public partial class UserWindow : ChildWindow {
		public User CurrentUser;
		public UserWindow() {
			InitializeComponent();
		}


		private void OKButton_Click(object sender, RoutedEventArgs e) {
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.editUserAsync(CurrentUser);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}

		public void init(User user) {
			CurrentUser = user;
			pnlData.DataContext = CurrentUser;
			GlobalContext.Single.Client.editUserCompleted += Client_editUserCompleted;
		}

		void Client_editUserCompleted(object sender, editUserCompletedEventArgs e) {
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
			GlobalContext.Single.Client.editUserCompleted -= Client_editUserCompleted;

		}
	}
}

