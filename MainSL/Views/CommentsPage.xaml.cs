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
using System.Windows.Navigation;
using MainSL.MainSVC;

namespace MainSL.Views {
	public partial class CommentsPage : Page {
		public static bool Inited { get; set; }
		public CommentsPage() {
			InitializeComponent();
			if (!Inited) {
				GlobalContext.Single.Client.FinishCommentTBPCompleted += Client_FinishCommentTBPCompleted;
				GlobalContext.Single.Client.GetCommentsListCompleted += Client_GetCommentsListCompleted;
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.GetCommentsListAsync();
			}
			Inited = true;
		}

		void Client_GetCommentsListCompleted(object sender, MainSVC.GetCommentsListCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			grdBlanks.ItemsSource = e.Result;
		}

		void Client_FinishCommentTBPCompleted(object sender, MainSVC.FinishCommentTBPCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			ReturnMessage msg = e.Result;
			MessageBox.Show(msg.Message);
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.GetCommentsListAsync();
		}

		void win_Closed(object sender, EventArgs e) {
			CommentWindow win = sender as CommentWindow;
			if (win.DialogResult == true) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.FinishCommentTBPAsync(win.CurrentComment);
			} else {
				win.CurrentComment.Finished = false;
			}
		}



		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {
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

	}
}
