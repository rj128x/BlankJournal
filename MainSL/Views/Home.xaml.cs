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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainSL {
	public partial class Home : Page {
		public Home() {
			InitializeComponent();
			GlobalContext.Single.onFinishLoadFolders += Single_onFinishLoadFolders;
			GlobalContext.Single.LoadFolders();
		}

		void Single_onFinishLoadFolders() {
			foreach (Folder folder in GlobalContext.Single.AllFolders.Values) {
				Button btn = new Button();
				btn.Content = folder.Name;
				btn.Height = 30;
				btn.Name = "btnFolder_" + folder.ID;
				pnlFolders.Children.Add(btn);
				btn.Click += btn_Click;
			}
		}

		void btn_Click(object sender, RoutedEventArgs e) {
			Button btn=sender as Button;
			int id = Int32.Parse(btn.Name.Replace("btnFolder_", ""));
			MessageBox.Show(id.ToString());
		}



		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {
			
		}

		private void Button_Click(object sender, RoutedEventArgs e) {

		}
	}
}