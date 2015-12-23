using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MainSL.MainSVC;
using System.ComponentModel;

namespace MainSL {	

	public class GlobalContext : DependencyObject {
		public static GlobalContext Single { get; protected set; }
		public static PropertyMetadata META = new PropertyMetadata(null);
		public static void init() {
			Single = new GlobalContext();
		}
		public GlobalContext() {
			CurrentUser = new UsersTable();
			CurrentUser.Name = "Noname";
		}

		public static readonly DependencyProperty CurrentUserProperty = DependencyProperty.Register("CurrentUser", typeof(UsersTable), typeof(GlobalContext), META);
		public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(GlobalContext), META);


		protected MainServiceClient Client;
		public bool IsBusy { 
			get { return (bool)GetValue(IsBusyProperty);}
			set {SetValue(IsBusyProperty,value);}
		}
				
		public UsersTable CurrentUser {
			get { return (UsersTable)GetValue(CurrentUserProperty); }
			set { SetValue(CurrentUserProperty, value); }
		}
		
		
		public void Connect() {
			Client = new MainServiceClient();
			IsBusy = true;
			Client.GetUserCompleted += Client_GetUserCompleted;
			Client.GetUserAsync();

		}

		void Client_GetUserCompleted(object sender, GetUserCompletedEventArgs e) {
			IsBusy = false;
			CurrentUser = e.Result;
			MessageBox.Show(String.Format("Добро пожаловать, {0}!", CurrentUser.Name));
		}
	}
}
