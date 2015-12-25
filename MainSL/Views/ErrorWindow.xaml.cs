﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace MainSL {
	public partial class ErrorWindow : ChildWindow {
		public ErrorWindow(Exception e) {
			InitializeComponent();
			if (e != null) {
				ErrorTextBox.Text = e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace;
			}
		}

		public ErrorWindow(Uri uri) {
			InitializeComponent();
			if (uri != null) {
				ErrorTextBox.Text = "Страница не найдена: \"" + uri.ToString() + "\"";
			}
		}

		public ErrorWindow(string message, string details) {
			InitializeComponent();
			ErrorTextBox.Text = message + Environment.NewLine + Environment.NewLine + details;
		}

		private void OKButton_Click(object sender, RoutedEventArgs e) {
			this.DialogResult = true;
		}
	}
}