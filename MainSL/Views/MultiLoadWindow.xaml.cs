using MainSL.MainSVC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

	public partial class MultiLoadWindow : ChildWindow {
		public Dictionary<string, byte[]> SelectedFiles;
		public KeyValuePair<string, byte[]> CurrentFile;
		public ObservableCollection<string> logData { get; set; }
		public ObservableCollection<string> fileNames { get; set; }
		public MultiLoadWindow() {
			GlobalContext.Single.Client.addFileCompleted += Client_addFileCompleted;
			SelectedFiles = new Dictionary<string,byte[]>();
			fileNames=new ObservableCollection<string>();
			logData = new ObservableCollection<string>();
			
			InitializeComponent();
			pnlData.DataContext = this;
		}

		void Client_addFileCompleted(object sender, MainSVC.addFileCompletedEventArgs e) {
			ReturnMessage msg = e.Result as ReturnMessage;
			logData.Add(msg.Message);
			SelectedFiles.Remove(CurrentFile.Key);
			fileNames.Remove(CurrentFile.Key);
			if (SelectedFiles.Count > 0) {
				CurrentFile = SelectedFiles.First();
				GlobalContext.Single.Client.addFileAsync(CurrentFile.Key, CurrentFile.Value);
			} else {
				GlobalContext.Single.IsBusy = false;
			}
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			if (SelectedFiles.Count > 0) {
				CurrentFile = SelectedFiles.First();
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.addFileAsync(CurrentFile.Key, CurrentFile.Value);
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}
		public void deinit() {
			GlobalContext.Single.Client.addFileCompleted -= Client_addFileCompleted;
		}

		protected override void OnClosed(EventArgs e) {
			deinit();
			base.OnClosed(e);
		}
		private void btnChoose_Click(object sender, RoutedEventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Файлы бланков |*.docx";
			dlg.Multiselect = true;
			if (dlg.ShowDialog() == true) {
				foreach (FileInfo file in dlg.Files) {
					FileStream str = file.OpenRead();
					byte[] buffer = new byte[str.Length];
					str.Read(buffer, 0, (int)str.Length);
					str.Close();
					string fileInfo = file.Name;
					SelectedFiles.Add(fileInfo, buffer);
					fileNames.Add(fileInfo);
				}
			}
		}
	}
}

