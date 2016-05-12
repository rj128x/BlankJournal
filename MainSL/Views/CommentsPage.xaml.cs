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
using System.ComponentModel;

namespace MainSL.Views {
	public partial class CommentsPage : Page, INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		private CommentAnswer _currentFilter;

		protected CommentAnswer CurrentFilter {
			get { return _currentFilter; }
			set {
				_currentFilter = value;
				NotifyChanged("CurrentFilter");
			}
		}

		public CommentsPage() {
			InitializeComponent();
			init();
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.GetCommentsListAsync(CurrentFilter);
		}

		public void init() {
			GlobalContext.Single.Client.GetCommentsListCompleted += Client_GetCommentsListCompleted;
		}

		public void deInit() {
			GlobalContext.Single.Client.GetCommentsListCompleted -= Client_GetCommentsListCompleted;
		}

		void Client_GetCommentsListCompleted(object sender, MainSVC.GetCommentsListCompletedEventArgs e) {
			GlobalContext.Single.IsBusy = false;
			CurrentFilter = e.Result as CommentAnswer;
			grdBlanks.ItemsSource = CurrentFilter.Data;
			pnlFilter.DataContext = CurrentFilter;
		}

		void win_Closed(object sender, EventArgs e) {
			CommentWindow win = sender as CommentWindow;
			//if (win.DialogResult == true) {
				GlobalContext.Single.IsBusy = true;
				GlobalContext.Single.Client.GetCommentsListAsync(CurrentFilter);
			//}
		}



		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e) {
			deInit();
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

		private void btnRefresh_Click(object sender, RoutedEventArgs e) {
			GlobalContext.Single.IsBusy = true;
			GlobalContext.Single.Client.GetCommentsListAsync(CurrentFilter);
		}

	}
}
