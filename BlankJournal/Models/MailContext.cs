using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace BlankJournal.Models {
	public class MailContext {

		public static bool SendComment(TBPComment comment) {
			try {
				Logger.info("оправка почты о новом замечании");
				List<string> mailTo = new List<string>();
				foreach (User user in DBContext.Single.AllUsers.Values) {
					if (!user.SendMailComment)
						continue;
					bool AvailFolder = false;
					try {
						BlanksEntities eni = new BlanksEntities();
						TBPInfoTable bl = (from t in eni.TBPInfoTable where t.ID == comment.TBPID select t).FirstOrDefault();
						if (user.AvailFoldersList.Contains(bl.Folder))
							AvailFolder = true;
					} catch (Exception e) {}
					if (!AvailFolder)
						continue;
					string[] mails = user.Mail.Split(new char[] { ';' });
					foreach (string mail in mails) {
						if (!string.IsNullOrEmpty(mail)) {
							mailTo.Add(mail);
						}
					}
				}
				string message = String.Format("<h3>Создано замечание к бланку № {0}<h3><br/>Текст комментария: {1} <br/> Автор замечения: {2} <br/>", 
					comment.TBPNumber, comment.CommentText,DBContext.Single.getUserByLogin(comment.Author).Name);
				message += String.Format("<h3><a href='{0}'>Перейти к списку ТБП</a></h3>", String.Format("http://{0}:{1}", HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port));

				SendMailLocal(Settings.Single.smtpServer, Settings.Single.smtpPort, Settings.Single.smtpUser,
					Settings.Single.smtpPassword, Settings.Single.smtpDomain, Settings.Single.smtpFrom, mailTo, "Новое замечание", message, true,comment.Data);
				return true;
			}
			catch (Exception e) {
				Logger.info("ошибка при отправке почты " + e.ToString());
				return false;
			}
		}

		private static bool SendMailLocal(string smtp_server, int port, string mail_user, string mail_password, string domain, string mail_from, List<string> mailToList, string subject, string message, bool is_html,byte[]data) {

			System.Net.Mail.MailMessage mess = new System.Net.Mail.MailMessage();

			mess.From = new MailAddress(mail_from);
			mess.Subject = subject; mess.Body = message;
			foreach (string mail in mailToList) {
				try {
					mess.To.Add(mail);
				}
				catch { }
			}

			if (data != null) {
				try {
					Attachment att = new Attachment(new MemoryStream(data), "file.docx");
					mess.Attachments.Add(att);
				}
				catch { }
			}

			mess.SubjectEncoding = System.Text.Encoding.UTF8;
			mess.BodyEncoding = System.Text.Encoding.UTF8;
			mess.IsBodyHtml = is_html;
			System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(smtp_server, port);
			client.EnableSsl = true;
			if (string.IsNullOrEmpty(mail_user)) {
				client.UseDefaultCredentials = true;
			}
			else {
				client.Credentials = new System.Net.NetworkCredential(mail_user, mail_password, domain);
			}
			// Отправляем письмо
			client.Send(mess);
			return true;
		}
	}
}