using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace BlankJournal.Models {
	public class MD5Class {
		public static string getString(byte[] data) {
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] hashenc = md5.ComputeHash(data);
			string result = "";
			foreach (var b in hashenc) {
				result += b.ToString("x2");
			}
			return result;
		}
	}
}