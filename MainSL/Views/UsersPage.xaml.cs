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
	public partial class UsersPage : Page {
		public UsersPage() {
			InitializeComponent();
			init();
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.getAllUsersAsync();
		}

		public void init() {
			GlobalContext.Single.Client.getAllUsersCompleted += Client_getAllUsersCompleted;
			GlobalContext.Single.Client.deleteUserCompleted += Client_deleteUserCompleted;
		}

		void Client_deleteUserCompleted(object sender, deleteUserCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			ReturnMessage msg = e.Result as ReturnMessage;
			MessageBox.Show(msg.Message);
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.getAllUsersAsync();

		}

		public void deinit() {
			GlobalContext.Single.Client.getAllUsersCompleted -= Client_getAllUsersCompleted;
			GlobalContext.Single.Client.deleteUserCompleted -= Client_deleteUserCompleted;
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
			User currentUser = grdUsers.SelectedItem as User;
			UserWindow newWindow = new UserWindow();
			currentUser.IsEditing = true;
			newWindow.init(currentUser);
			newWindow.Closed+=newWindow_Closed;
			newWindow.Show();
		}

		private void btnNewUser_Click(object sender, RoutedEventArgs e) {
			UserWindow newWindow = new UserWindow();
			User user=new User();
			user.IsEditing=false;
			newWindow.init(user);
			newWindow.Closed += newWindow_Closed;
			newWindow.Show();
		}

		void newWindow_Closed(object sender, EventArgs e) {
			UserWindow win=sender as UserWindow;
			//if (win.DialogResult == true) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.getAllUsersAsync();
			//}
		}

		private void btnDel_Click(object sender, RoutedEventArgs e) {
			User currentUser = grdUsers.SelectedItem as User;
			if (MessageBox.Show("Вы уверены что хотите удалить пользователя " + currentUser.Name, "Удаление", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.deleteUserAsync(currentUser);
			}
		}

	}
}
