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

namespace MainSL.Views {
	public partial class CommentsPage : Page {
		public static bool Inited { get; set; }
		public CommentsPage() {
			InitializeComponent();
			if (!Inited) {
				GlobalContext.Single.Client.CreateCommentTBPCompleted += Client_CreateCommentTBPCompleted;
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
			throw new NotImplementedException();
		}

		void Client_CreateCommentTBPCompleted(object sender, MainSVC.CreateCommentTBPCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			throw new NotImplementedException();
		}


		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {
		}

		private void btnShow_Click(object sender, RoutedEventArgs e) {

		}

		private void btnFinish_Click(object sender, RoutedEventArgs e) {

		}

	}
}
