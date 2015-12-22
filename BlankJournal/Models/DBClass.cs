using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BlankJournal {
	public class DBClass {
		public static SqlConnection getConnection() {
			string conStr=String.Format(@"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};Connection Timeout={4}",
				Settings.Single.DBServer,Settings.Single.DBName,Settings.Single.DBUser,Settings.Single.DBPassword, 3);
			SqlConnection con = new SqlConnection(conStr);
			return con;
		}

		public static Dictionary<string, User> getAllUsers() {
			Dictionary<string, User> users = new Dictionary<string, User>();
			SqlConnection con = getConnection();
			string commandText = "SELECT * FROM USERS";
			SqlCommand command = con.CreateCommand();
			command.CommandText = commandText;
			try {
				con.Open();
				SqlDataReader reader = command.ExecuteReader();
				while (reader.Read()) {
					User user = new User();
					user.UserLogin = reader.GetString(reader.GetOrdinal("UserLogin"));
					user.UserName = reader.GetString(reader.GetOrdinal("UserName"));
					users.Add(user.UserLogin, user);
				}
			} catch (Exception e) {
				Logger.info("ошибка при получении списка пользоватлей");
				Logger.error(e.ToString());
			} finally {
				try { con.Close(); } catch { }
			}
			return users;
		}
	}
}