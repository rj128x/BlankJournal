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
	public partial class FoldersPage : Page {
		public FoldersPage() {
			InitializeComponent();
			init();
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.getFoldersListAsync();
		}

		public void init() {
			GlobalContext.Single.Client.getFoldersListCompleted += Client_getFoldersListCompleted;
			GlobalContext.Single.Client.removeFolderCompleted += Client_removeFolderCompleted;			
		}

		private void Client_removeFolderCompleted(object sender, removeFolderCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			ReturnMessage msg = e.Result as ReturnMessage;
			MessageBox.Show(msg.Message);
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.getFoldersListAsync();			
		}

		private void Client_getFoldersListCompleted(object sender, getFoldersListCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			grdFolders.ItemsSource = e.Result;
			GlobalContext.Single.RefreshFolders(e.Result);
		}

		public void deinit() {
			GlobalContext.Single.Client.getFoldersListCompleted -= Client_getFoldersListCompleted;
			GlobalContext.Single.Client.removeFolderCompleted -= Client_removeFolderCompleted;			
		}
		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {
		}
		protected override void OnNavigatedFrom(NavigationEventArgs e) {			
			deinit();		
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e) {
			Folder currentFolder = grdFolders.SelectedItem as Folder;
			FolderWindow newWindow = new FolderWindow();
			newWindow.edit = true;
			newWindow.init(currentFolder);
			newWindow.Closed += newWindow_Closed;
			newWindow.Show();
		}

		void newWindow_Closed(object sender, EventArgs e) {
			FolderWindow win=sender as FolderWindow;
			//if (win.DialogResult == true) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.getFoldersListAsync();
			//}
		}

		private void btnDel_Click(object sender, RoutedEventArgs e) {
			Folder currentFolder = grdFolders.SelectedItem as Folder;
			if (MessageBox.Show("Все бланки папки будут перенесены в папку 'Удаленные'\r\nВы уверены что хотите удалить папку " + currentFolder.ID, "Удаление", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.removeFolderAsync(currentFolder);
			}
		}

		private void btnNewFoder_Click(object sender, RoutedEventArgs e) {
			FolderWindow newWindow = new FolderWindow();
			Folder folder = new Folder();
			folder.ID = "";
			folder.Name = "";
			newWindow.init(folder);
			newWindow.edit = false;
			newWindow.Closed += newWindow_Closed;
			newWindow.Show();
		}
	}
}
