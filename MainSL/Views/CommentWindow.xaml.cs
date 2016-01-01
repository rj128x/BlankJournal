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
	public partial class CommentWindow : ChildWindow {
		public TBPComment CurrentComment{get;set;}
		public CommentWindow() {
			InitializeComponent();
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			GlobalContext.Single.IsBusy = true;
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
				FileStream str = dlg.File.OpenRead();
				byte[] buffer = new byte[str.Length];
				str.Read(buffer, 0, (int)str.Length);
				str.Close();
				CurrentComment.FileInfoData = dlg.File.Name;
				CurrentComment.Data = buffer;
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
	}
}

