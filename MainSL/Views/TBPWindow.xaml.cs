using MainSL.MainSVC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MainSL {
	public partial class TBPWindow : ChildWindow {
		public string EditingFileName { get; set; }
		public TBPWindow() {
			InitializeComponent();
		}
		public TBPInfo CurrentBlank { get; set; }

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			if (!String.IsNullOrEmpty(EditingFileName)) {
				try {
					CurrentBlank.UpdatedWord = true;
					CurrentBlank.WordData = File.ReadAllBytes(EditingFileName);
				} catch { }
			}
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.CreateTBPAsync(CurrentBlank);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}

		protected override void OnClosed(EventArgs e) {
			deinit();
			base.OnClosed(e);
		}

		public void Init(TBPInfo blank) {
			CurrentBlank = blank;
			LayoutRoot.DataContext = blank;
			GlobalContext.Single.Client.CreateTBPCompleted += Client_CreateTBPCompleted;
			GlobalContext.Single.Client.getDataRecordCompleted += Client_getDataRecordCompleted;
		}

		public void deinit() {
			GlobalContext.Single.Client.CreateTBPCompleted -= Client_CreateTBPCompleted;
			GlobalContext.Single.Client.getDataRecordCompleted -= Client_getDataRecordCompleted;
		}

		void Client_CreateTBPCompleted(object sender, CreateTBPCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			ReturnMessage msg = e.Result as ReturnMessage;
			MessageBox.Show(msg.Message);
			if (msg.Result == true) {				
				this.DialogResult = true;
			}
		}

		private void btnChoosePDF_Click(object sender, RoutedEventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Файлы pdf |*.pdf";
			if (dlg.ShowDialog() == true) {
				FileStream str = dlg.File.OpenRead();
				byte[]buffer=new byte[str.Length];
				str.Read(buffer, 0, (int)str.Length);
				str.Close();
				CurrentBlank.PDFData = buffer;
				CurrentBlank.FileInfoPDF = String.Format("{0}", dlg.File.Name);
				CurrentBlank.UpdatedPDF = true;
			}			
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
				CurrentBlank.UpdatedWord = true;
				EditingFileName = null;
			}
		}

		private void btnEdit_Click(object sender, RoutedEventArgs e) {
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
			if (e.Result != null) {
				DataRecord rec = e.Result as DataRecord;
				CurrentBlank.WordData = rec.Data;
				editFile();
			}
		}

		protected void editFile() {
			if (CurrentBlank.WordData != null) {
				try {
					string str = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TempTBP";
					if (!Directory.Exists(str))
						Directory.CreateDirectory(str);
					str = string.Format("{0}\\{1}_{2}", str, DateTime.Now.ToString("yyyyMMddhhmmss"), CurrentBlank.FileInfoWord);
					EditingFileName = str;
					File.WriteAllBytes(str, CurrentBlank.WordData);
					WebBrowserBridge.OpenURL(new Uri("file://" + str), "_blank");
				} catch {

				}
			}
		}

						
	}
}

