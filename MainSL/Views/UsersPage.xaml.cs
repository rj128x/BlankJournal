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

namespace MainSL.Views {
	public partial class UsersPage : Page {
		public UsersPage() {
			InitializeComponent();
			init();
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.getAllUsersAsync();
		}

		public void init() {
			GlobalContext.Single.Client.getAllUsersCompleted += Client_getAllUsersCompleted;
		}

		public void deinit() {
			GlobalContext.Single.Client.getAllUsersCompleted -= Client_getAllUsersCompleted;
		}

		void Client_getAllUsersCompleted(object sender, MainSVC.getAllUsersCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			grdUsers.ItemsSource = e.Result;		
		}

		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {
		}
		protected override void OnNavigatedFrom(NavigationEventArgs e) {
			deinit();		
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e) {

		}

	}
}
