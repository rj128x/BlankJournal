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
using System.Windows.Shapes;
using System.ComponentModel;

namespace MainSL.Views
{
	public partial class CommentsWindow : ChildWindow
	{
		public CommentAnswer CurrentFilter;

		public CommentsWindow() {
			InitializeComponent();
			init();
		}

		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			deInit();
		}		

		public void init() {
			GlobalContext.Single.Client.GetCommentsListCompleted += Client_GetCommentsListCompleted;
		}

		public void deInit() {
			GlobalContext.Single.Client.GetCommentsListCompleted -= Client_GetCommentsListCompleted;
		}

		public void load() {
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.GetCommentsListAsync(CurrentFilter);
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}

		private void btnShow_Click(object sender, RoutedEventArgs e) {
			TBPComment comment = grdBlanks.SelectedItem as TBPComment;
			FloatWindow.OpenWindow("Home/getFile?id=" + comment.DataID);
		}
		
		private void btnFinish_Click(object sender, RoutedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			TBPComment current = grdBlanks.SelectedItem as TBPComment;
			current.Finished = true;
			CommentWindow win = new CommentWindow();
			win.Init(current);
			win.Closed += win_Closed;
			win.Show();
		}

		void win_Closed(object sender, EventArgs e) {
			CommentWindow win = sender as CommentWindow;
			//if (win.DialogResult == true) {
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.GetCommentsListAsync(CurrentFilter);
			//}
		}

		private void Client_GetCommentsListCompleted(object sender, GetCommentsListCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			grdBlanks.ItemsSource = e.Result.Data;
		}
	}
}

