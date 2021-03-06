﻿using MainSL.MainSVC;
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
	public partial class CommentWindow : ChildWindow {
		public string EditingFileName { get; set; }
		public TBPComment CurrentComment{get;set;}
		public CommentWindow() {
			InitializeComponent();
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			GlobalContext.Single.IsBusy = true;
			if (!String.IsNullOrEmpty(EditingFileName)) {
				try {
					CurrentComment.Data = File.ReadAllBytes(EditingFileName);
				} catch { }
			}
			if (!CurrentComment.Finished)
				GlobalContext.Single.Client.CreateCommentTBPAsync(CurrentComment);
			else
				GlobalContext.Single.Client.FinishCommentTBPAsync(CurrentComment);
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}

		private void btnChooseWord_Click(object sender, RoutedEventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Файлы docx |*.docx";
			if (dlg.ShowDialog() == true) {
				if (!dlg.File.Extension.ToLower().Contains(".docx")) {
					MessageBox.Show("Выбран файл неверного формата. Необходим формат docx");
					return;
				}
				FileStream str = dlg.File.OpenRead();
				byte[] buffer = new byte[str.Length];
				str.Read(buffer, 0, (int)str.Length);
				str.Close();
				CurrentComment.FileInfoData = dlg.File.Name;
				CurrentComment.Data = buffer;
				EditingFileName = null;
			}
		}

		public void Init(TBPComment comment) {
			CurrentComment = comment;
			pnlData.DataContext = comment;
			GlobalContext.Single.Client.CreateCommentTBPCompleted += Client_CreateCommentTBPCompleted;
			GlobalContext.Single.Client.FinishCommentTBPCompleted += Client_FinishCommentTBPCompleted;
		}

		void Client_FinishCommentTBPCompleted(object sender, FinishCommentTBPCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			ReturnMessage msg = e.Result as ReturnMessage;
			MessageBox.Show(msg.Message);
			if (msg.Result) {
				this.DialogResult = true;
			}
		}

		void Client_CreateCommentTBPCompleted(object sender, CreateCommentTBPCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			ReturnMessage msg = e.Result as ReturnMessage;
			MessageBox.Show(msg.Message);
			if (msg.Result) {
				this.DialogResult = true;
			}
		}

		public void deinit() {
			GlobalContext.Single.Client.CreateCommentTBPCompleted -= Client_CreateCommentTBPCompleted;
			GlobalContext.Single.Client.FinishCommentTBPCompleted -= Client_FinishCommentTBPCompleted;
		}

		protected override void OnClosed(EventArgs e) {
			deinit();
			base.OnClosed(e);
		}

		private void btnEditWord_Click(object sender, RoutedEventArgs e) {
			if (!String.IsNullOrEmpty(EditingFileName)) {
				WebBrowserBridge.OpenURL(new Uri("file://" + EditingFileName), "_blank");
				return;
			}
			if (CurrentComment.Data != null) {
				try {
					string str = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TempTBP";
					if (!Directory.Exists(str))
						Directory.CreateDirectory(str);
					str = string.Format("{0}\\{1}_{2}", str, DateTime.Now.ToString("yyyyMMddhhmmss"), CurrentComment.FileInfoData);
					EditingFileName = str;
					File.WriteAllBytes(str, CurrentComment.Data);
					WebBrowserBridge.OpenURL(new Uri("file://" + str), "_blank");
				} catch {

				}
			}
		}

	}
}

