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
			this.DialogResult = true;
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
				txtWord.Text = "Файл выбран";
				CurrentComment.Data = buffer;
			}
		}

		public void Init(TBPComment comment) {
			CurrentComment = comment;
			pnlData.DataContext = comment;
		}
	}
}

