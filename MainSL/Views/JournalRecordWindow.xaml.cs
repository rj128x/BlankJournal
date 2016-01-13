using MainSL.MainSVC;
using System;
using System.Collections.Generic;
using System.IO;
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
	public partial class JournalRecordWindow : ChildWindow {
		public string EditingFileName { get; set; }
		public JournalRecord CurrentBlank { get; set; }
		public JournalRecordWindow() {
			InitializeComponent();
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			GlobalContext.Single.IsBusy = true;
			if (!String.IsNullOrEmpty(EditingFileName)) {
				try {
					CurrentBlank.WordData = File.ReadAllBytes(EditingFileName);
				} catch { }
			}
			if (CurrentBlank.isInit)
				GlobalContext.Single.Client.CreateBPAsync(CurrentBlank);
			else {
				GlobalContext.Single.Client.FinishBPAsync(CurrentBlank);
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}

		public void Init(JournalRecord rec) {
			CurrentBlank = rec;
			pnlData.DataContext = rec;
			GlobalContext.Single.Client.CreateBPCompleted += Client_CreateBPCompleted;
			GlobalContext.Single.Client.FinishBPCompleted += Client_FinishBPCompleted;
			GlobalContext.Single.Client.getDataRecordCompleted += Client_getDataRecordCompleted;
		}



		void Client_FinishBPCompleted(object sender, FinishBPCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			ReturnMessage msg = e.Result as ReturnMessage;
			MessageBox.Show(msg.Message);
			if (msg.Result) {
				this.DialogResult = true;
			}
		}

		void Client_CreateBPCompleted(object sender, CreateBPCompletedEventArgs e) {
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
			GlobalContext.Single.Client.CreateBPCompleted -= Client_CreateBPCompleted;
			GlobalContext.Single.Client.FinishBPCompleted -= Client_FinishBPCompleted;
			GlobalContext.Single.Client.getDataRecordCompleted -= Client_getDataRecordCompleted;
		}

		private void btnChooseWord_Click(object sender, RoutedEventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Файлы docx |*.docx";
			if (dlg.ShowDialog() == true) {
				FileStream str = dlg.File.OpenRead();
				byte[] buffer = new byte[str.Length];
				str.Read(buffer, 0, (int)str.Length);
				str.Close();
				CurrentBlank.WordData = buffer;
				CurrentBlank.FileInfoWord = String.Format("{0}", dlg.File.Name);
				EditingFileName = null;
			}
		}

		private void btnEditWord_Click(object sender, RoutedEventArgs e) {
			if (!String.IsNullOrEmpty(EditingFileName)) {
				WebBrowserBridge.OpenURL(new Uri("file://" + EditingFileName), "_blank");
				return;
			}
			if (CurrentBlank.WordData == null) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.getDataRecordAsync(CurrentBlank.IDWordData);
			} else {
				editFile();
			}
		}

		void Client_getDataRecordCompleted(object sender, getDataRecordCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			if (e.Result!=null) {
				DataRecord rec = e.Result as DataRecord;
				CurrentBlank.WordData = rec.Data;
				editFile();
			} 
		}

		protected void editFile() {
			if (CurrentBlank.WordData != null) {
				try {					
					string str = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+"\\TempTBP";
					if (!Directory.Exists(str))
						Directory.CreateDirectory(str);
					str = string.Format("{0}\\{1}_{2}", str, DateTime.Now.ToString("yyyyMMddhhmmss"), CurrentBlank.FileInfoWord);
					EditingFileName = str;
					File.WriteAllBytes(str, CurrentBlank.WordData);
					WebBrowserBridge.OpenURL(new Uri("file://"+str), "_blank");
				}catch{
					
				}
			}
		}

	}
}

