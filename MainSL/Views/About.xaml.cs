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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainSL {
	public partial class About : Page {
		public About() {
			InitializeComponent();
			init();
			GlobalContext.Single.Client.getOperationsInfoAsync();
		}

		public void init() {
			GlobalContext.Single.Client.getOperationsInfoCompleted += Client_getOperationsInfoCompleted;
		}
		public void deInit() {
			GlobalContext.Single.Client.getOperationsInfoCompleted -= Client_getOperationsInfoCompleted;
		}

		void Client_getOperationsInfoCompleted(object sender, MainSVC.getOperationsInfoCompletedEventArgs e) {
			ReturnMessage msg = e.Result;
			pnlData.DataContext = msg;
		}

		// Выполняется, когда пользователь переходит на эту страницу.
		protected override void OnNavigatedTo(NavigationEventArgs e) {

		}

		protected override void OnNavigatedFrom(NavigationEventArgs e) {
			deInit();
		}
	}
}