using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	public partial class InstallWindow : ChildWindow, INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyChanged(string propName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		protected bool _installed;
		public bool Installed {
			get {
				return _installed;
			}
			set {
				_installed = value;
				NotifyChanged("Installed");
			}
		}
		public InstallWindow() {
			InitializeComponent();
			LayoutRoot.DataContext = this;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			if (!Installed) {
				bool install = Application.Current.Install();
				this.DialogResult = install;
			}

			this.DialogResult = true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = false;
		}
	}
}

