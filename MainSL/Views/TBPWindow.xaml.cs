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
		public TBPWindow() {
			InitializeComponent();
		}
		public TBPInfo CurrentBlank { get; set; }

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}

		public void Init(TBPInfo blank) {
			CurrentBlank = blank;
			LayoutRoot.DataContext = blank;
		}

		private void btnChoosePDF_Click(object sender, RoutedEventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			if (dlg.ShowDialog() == true) {
				FileStream str = dlg.File.OpenRead();
				byte[]buffer=new byte[str.Length];
				str.Read(buffer, 0, (int)str.Length);
				str.Close();
				txtPDF.Text = "Файл выбран";
				CurrentBlank.PDFData = buffer;
				CurrentBlank.UpdatedPDF = true;
			}			
		}

		private void btnChooseWord_Click(object sender, RoutedEventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			if (dlg.ShowDialog() == true) {
				FileStream str = dlg.File.OpenRead();
				byte[] buffer = new byte[str.Length];
				str.Read(buffer, 0, (int)str.Length);
				str.Close();
				txtWord.Text = "Файл выбран";
				CurrentBlank.WordData = buffer;
				CurrentBlank.UpdatedWord = true;
			}
		}

						
	}
}

