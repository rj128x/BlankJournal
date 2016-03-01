﻿using MainSL.MainSVC;
using MainSL.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
	public partial class Home : Page, INotifyPropertyChanged
	{		
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		protected TBPInfo _currentTBP;
		public TBPInfo CurrentTBP {
			get {
				return _currentTBP;
			}
			set {
				_currentTBP = value;
				NotifyChanged("CurrentTBP");
			}
		}

		public Home() {
			InitializeComponent();
			init();
			this.DataContext = this;
		}

		public void init() {
			GlobalContext.Single.Client.GetTBPBlanksByFolderCompleted += Client_GetTBPBlanksByFolderCompleted;
			GlobalContext.Single.Client.InitOBPCompleted += Client_InitOBPCompleted;
			GlobalContext.Single.Client.InitTBPCompleted += Client_InitTBPCompleted;
			GlobalContext.Single.Client.removeTBPCompleted += Client_removeTBPCompleted;
			GlobalContext.Single.Client.InitCommentCompleted += Client_InitCommentCompleted;
			GlobalContext.Single.Client.GetJournalBPCompleted += Client_GetJournalBPCompleted;

		}

		

		public void deInit() {
			GlobalContext.Single.Client.GetTBPBlanksByFolderCompleted -= Client_GetTBPBlanksByFolderCompleted;
			GlobalContext.Single.Client.InitOBPCompleted -= Client_InitOBPCompleted;
			GlobalContext.Single.Client.InitTBPCompleted -= Client_InitTBPCompleted;
			GlobalContext.Single.Client.removeTBPCompleted -= Client_removeTBPCompleted;
			GlobalContext.Single.Client.InitCommentCompleted -= Client_InitCommentCompleted;
			GlobalContext.Single.Client.GetJournalBPCompleted -= Client_GetJournalBPCompleted;
		}

		void Client_GetTBPBlanksByFolderCompleted(object sender, GetTBPBlanksByFolderCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			grdTBPBlanks.ItemsSource = e.Result;
		}


		public Folder CurrentFolder;

		void btn_Click(object sender, RoutedEventArgs e) {
			Button btn = sender as Button;
			string id = btn.Name.Replace("btnFolder_", "");
			CurrentFolder = GlobalContext.Single.AllFolders[id];
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.GetTBPBlanksByFolderAsync(CurrentFolder.ID);
		}



		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {
			foreach (Folder folder in GlobalContext.Single.AllFolders.Values) {
				Button btn = new Button();
				btn.Content = folder.ID+" "+folder.Name;
				btn.Height = 30;
				btn.Name = "btnFolder_" + folder.ID;
				btn.Click += btn_Click;
				pnlFolders.Children.Add(btn);
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e) {
			deInit();
			base.OnNavigatedFrom(e);
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			if (CurrentFolder != null) {
				TBPInfo newBlank = new TBPInfo();
				newBlank.Number = CurrentFolder.ID + "-";
				newBlank.Name = "";
				newBlank.FolderID = CurrentFolder.ID;
				TBPWindow newWindow = new TBPWindow();
				newBlank.EditingTBP = false;
				newWindow.Init(newBlank);
				newWindow.Closed += newWindow_Closed;
				newWindow.Show();
			}
		}

		void newWindow_Closed(object sender, EventArgs e) {
			TBPWindow win = sender as TBPWindow;
			//if (win.DialogResult == true) {
			GlobalContext.Single.IsBusy = true;
			if (CurrentFolder != null) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.GetTBPBlanksByFolderAsync(CurrentFolder.ID);
			}
			//} 
		}

		private void btnUseNextTBP_Click(object sender, RoutedEventArgs e) {
			if (CurrentTBP != null) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.InitTBPAsync(CurrentTBP);
			}
		}

		private void btnUseOBP_Click(object sender, RoutedEventArgs e) {
			if (CurrentTBP != null) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.InitOBPAsync(CurrentTBP);
			}
		}

		void Client_InitOBPCompleted(object sender, InitOBPCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			JournalRecord newBlank = e.Result as JournalRecord;
			newBlank.isInit = true;
			JournalRecordWindow win = new JournalRecordWindow();
			win.Init(newBlank);
			win.Closed += win_Closed;
			win.Show();
		}

		void Client_InitTBPCompleted(object sender, InitTBPCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			JournalRecord newBlank = e.Result as JournalRecord;
			newBlank.isInit = true;
			JournalRecordWindow win = new JournalRecordWindow();
			win.Init(newBlank);
			win.Closed += win_Closed;
			win.Show();
		}

		void win_Closed(object sender, EventArgs e) {
			JournalRecordWindow win = sender as JournalRecordWindow;
			//if (win.DialogResult == true) {
			if (CurrentFolder != null) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.GetTBPBlanksByFolderAsync(CurrentFolder.ID);
			}
			//}
		}

		private void btnNewOBP_Click(object sender, RoutedEventArgs e) {
			TBPInfo tbp = new TBPInfo();
			tbp.Number = "-";
			tbp.Name = " ";
			tbp.FolderID = "-";
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.InitOBPAsync(tbp);
		}

		private void btnShowPDF_Click(object sender, RoutedEventArgs e) {
			if (CurrentTBP != null) {
				FloatWindow.OpenWindow("Home/getFile?id=" + CurrentTBP.IDPDFData);
			}
		}

		private void btnShowWord_Click(object sender, RoutedEventArgs e) {
			if (CurrentTBP != null) {
				FloatWindow.OpenWindow("Home/getFile?id=" + CurrentTBP.IDWordData);
			}
		}

		private void btnEditTBP_Click(object sender, RoutedEventArgs e) {
			if (CurrentTBP != null) {
				TBPWindow newWindow = new TBPWindow();
				CurrentTBP.EditingTBP = true;
				newWindow.Init(CurrentTBP);
				newWindow.Closed += newWindow_Closed;
				newWindow.Show();
			}
		}

		private void btnCommentTBP_Click(object sender, RoutedEventArgs e) {
			if (CurrentTBP != null) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.InitCommentAsync(CurrentTBP);
			}
		}

		void Client_InitCommentCompleted(object sender, InitCommentCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			TBPComment com = e.Result as TBPComment;
			if (com != null) {
				CommentWindow commentWin = new CommentWindow();
				commentWin.Init(com);
				commentWin.Closed += commentWin_Closed;
				commentWin.Show();
			}
		}


		void commentWin_Closed(object sender, EventArgs e) {
			CommentWindow win = sender as CommentWindow;
			//if (win.DialogResult == true) {
			if (CurrentFolder != null) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.GetTBPBlanksByFolderAsync(CurrentFolder.ID);
			}
			//}
		}

		private void btnShowWordOBP_Click(object sender, RoutedEventArgs e) {
			if (CurrentTBP != null) {
				FloatWindow.OpenWindow("Home/getOBPWord?id=" + CurrentTBP.Number);
			}
		}

		private void grdTBPBlanks_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			CurrentTBP = grdTBPBlanks.SelectedItem as TBPInfo;
		}

		private void btnTemplateOBP_Click(object sender, RoutedEventArgs e) {
			FloatWindow.OpenWindow("/Data/EmptyOBP.docx");
		}

		private void btnHistoryTBP_Click(object sender, RoutedEventArgs e) {
			if (CurrentTBP != null) {
				TBPHistoryWindow win = new TBPHistoryWindow();
				win.init(CurrentTBP);
				win.Show();
			}
		}

		private void btnPacketLoad_Click(object sender, RoutedEventArgs e) {
			MultiLoadWindow multiWin = new MultiLoadWindow();
			multiWin.Show();
		}

		private void btnDeleteTBP_Click(object sender, RoutedEventArgs e) {
			if (CurrentTBP != null) {
				if (MessageBox.Show(String.Format("Вы уверены что хотите удалить бланк {0}?", CurrentTBP.Number), "Удаление бланка",
					MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
					GlobalContext.Single.IsBusy = true;
					GlobalContext.Single.Client.removeTBPAsync(CurrentTBP);

				}
			}
		}

		void Client_removeTBPCompleted(object sender, removeTBPCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			ReturnMessage msg = e.Result as ReturnMessage;
			MessageBox.Show(msg.Message);
			if (CurrentFolder != null) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.GetTBPBlanksByFolderAsync(CurrentFolder.ID);
			}
		}

		void Client_GetJournalBPCompleted(object sender, GetJournalBPCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			TBPJournalWindow win = new TBPJournalWindow();
			foreach (JournalRecord rec in e.Result.Data) {
				rec.Closed = true;
			}
			win.cntrlJournal.grdBlanks.ItemsSource = e.Result.Data;
			win.Show();
		}

		private void btnJournalTBP_Click(object sender, RoutedEventArgs e) {
			GlobalContext.Single.IsBusy = true;
			JournalAnswer Filter = new JournalAnswer();
			Filter.dateStart = new DateTime(DateTime.Now.Year, 1, 1);
			Filter.dateEnd = DateTime.Now.Date.AddDays(1);
			Filter.tbpNumber = CurrentTBP.Number;
			GlobalContext.Single.Client.GetJournalBPAsync(Filter);
		}

		private void btnSync_Click(object sender, RoutedEventArgs e) {
			if (MessageBox.Show("Будет выполнено принудительное копирование всех бланков из БД в файлы базы ОС (AutoArchive). Вы уверены?","Синхронизация", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
				FloatWindow.OpenWindow("/Home/SyncDB");
			}
		}
	}
}