using System;
using System.Collections.Generic;
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
					string[] mails = user.Mail.Split(new char[] { ';' });
					foreach (string mail in mails) {
						if (!string.IsNullOrEmpty(mail)) {
							mailTo.Add(mail);
						}
					}
				}
				string message = String.Format("<h3>Создано замечание к бланку № {0}<h3><br/>Текст комментария: {1} <br/> Автор замечания: {2}",
					comment.TBPNumber, comment.CommentText, comment.CommentPerform);
				SendMailLocal(Settings.Single.smtpServer, Settings.Single.smtpPort, Settings.Single.smtpUser, 
					Settings.Single.smtpPassword,Settings.Single.smtpDomain,Settings.Single.smtpFrom, mailTo, "Новое замечание", message, true);
				return true;
			} catch (Exception e) {
				Logger.info("ошибка при отправке почты " + e.ToString());
				return false;
			}
		}

		private static bool SendMailLocal(string smtp_server, int port, string mail_user, string mail_password, string domain, string mail_from, List<string> mailToList, string subject, string message, bool is_html) {

			System.Net.Mail.MailMessage mess = new System.Net.Mail.MailMessage();

			mess.From = new MailAddress(mail_from);
			mess.Subject = subject; mess.Body = message;
			foreach (string mail in mailToList) {
				try {
					mess.To.Add(mail);
				} catch { }
			}

			mess.SubjectEncoding = System.Text.Encoding.UTF8;
			mess.BodyEncoding = System.Text.Encoding.UTF8;
			mess.IsBodyHtml = is_html;
			System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(smtp_server, port);
			client.EnableSsl = true;
			if (string.IsNullOrEmpty(mail_user)) {
				client.UseDefaultCredentials = true;
			} else {
				client.Credentials = new System.Net.NetworkCredential(mail_user, mail_password, domain);
			}
			// Отправляем письмо
			client.Send(mess);
			return true;
		}
	}
}