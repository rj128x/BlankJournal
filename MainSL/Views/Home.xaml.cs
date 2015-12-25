using MainSL.MainSVC;
using MainSL.Views;
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

		public static bool inited { get; set; }
		public Home() {
			InitializeComponent();
			
				GlobalContext.Single.Client.GetTBPBlanksByFolderCompleted += Client_GetTBPBlanksByFolderCompleted;
			if (!inited) {	
				GlobalContext.Single.Client.InitOBPCompleted += Client_InitOBPCompleted;
				GlobalContext.Single.Client.InitTBPCompleted += Client_InitTBPCompleted;
				GlobalContext.Single.Client.CreateBPCompleted += Client_CreateBPCompleted;
			}
			inited = true;
		}

		
		
		void Client_GetTBPBlanksByFolderCompleted(object sender, GetTBPBlanksByFolderCompletedEventArgs e) {
			grdTBPBlanks.ItemsSource = e.Result;
		}
		

		public Folder CurrentFolder;

		void btn_Click(object sender, RoutedEventArgs e) {
			Button btn=sender as Button;
			int id = Int32.Parse(btn.Name.Replace("btnFolder_", ""));
			//MessageBox.Show(id.ToString());
			CurrentFolder = GlobalContext.Single.AllFolders[id];
			GlobalContext.Single.Client.GetTBPBlanksByFolderAsync(id);
		}



		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {
			foreach (Folder folder in GlobalContext.Single.AllFolders.Values) {
				Button btn = new Button();
				btn.Content = folder.Name;
				btn.Height = 30;
				btn.Name = "btnFolder_" + folder.ID;
				btn.Click += btn_Click;
				pnlFolders.Children.Add(btn);
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			TBPInfo newBlank = new TBPInfo();
			newBlank.Number = "-";
			newBlank.Name = "";
			newBlank.FolderID = CurrentFolder.ID;
			TBPWindow newWindow = new TBPWindow();
			newWindow.Init(newBlank);
			newWindow.Show();
		}

		private void btnUseNextTBP_Click(object sender, RoutedEventArgs e) {
			TBPInfo tbp = grdTBPBlanks.SelectedItem as TBPInfo;
			GlobalContext.Single.Client.InitTBPAsync(tbp);
		}

		private void btnUseOBP_Click(object sender, RoutedEventArgs e) {
			TBPInfo tbp = grdTBPBlanks.SelectedItem as TBPInfo;
			GlobalContext.Single.Client.InitOBPAsync(tbp);
		}

		void Client_InitOBPCompleted(object sender, InitOBPCompletedEventArgs e) {
			JournalRecord newBlank = e.Result as JournalRecord;
			JournalRecordWindow win = new JournalRecordWindow();
			win.Init(newBlank);
			win.Closed += win_Closed;
			win.Show();
		}
		
		void Client_InitTBPCompleted(object sender, InitTBPCompletedEventArgs e) {
			JournalRecord newBlank = e.Result as JournalRecord;
			JournalRecordWindow win = new JournalRecordWindow();
			win.Init(newBlank);
			win.Closed += win_Closed;
			win.Show();			
		}

		void win_Closed(object sender, EventArgs e) {
			JournalRecordWindow win = sender as JournalRecordWindow;
			if (win.DialogResult==true){
				GlobalContext.Single.Client.CreateBPAsync(GlobalContext.Single.NewBPRecord);
			}
		}

		void Client_CreateBPCompleted(object sender, CreateBPCompletedEventArgs e) {
			ReturnMessage msg = e.Result as ReturnMessage;
			MessageBox.Show(msg.Message);
		}

		private void btnNewOBP_Click(object sender, RoutedEventArgs e) {
			TBPInfo tbp = new TBPInfo();
			tbp.Number = "-";
			tbp.Name = "ОБП";

			GlobalContext.Single.Client.InitOBPAsync(tbp);
		}

		



	}
}