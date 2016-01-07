using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class TBPComment {
		public string ID { get; set; }
		public string TBPNumber { get; set; }
		public int TBPID { get; set; }
		public string Author { get; set; }
		public string CommentText { get; set; }
		public DateTime DateCreate { get; set; }
		public string Performer { get; set; }
		public DateTime DatePerform { get; set; }
		public bool Finished { get; set; }
		public string DataID { get; set; }
		public string FileInfoData { get; set; }
		public byte[] Data { get; set; }
		public string CommentPerform { get; set; }

		public TBPComment() { }

		public TBPComment(TBPCommentsTable tbl){
			ID = tbl.Id;
			TBPNumber = tbl.TBPNumber;
			TBPID = tbl.TBPID;
			Author = DBContext.Single.getUserByLogin(tbl.Author).Name;
			CommentText = tbl.Comment;
			DateCreate = tbl.DateCreate;
			Finished = tbl.Finished;
			CommentPerform = tbl.CommentPerform;
			DataID = tbl.WordData;
			if (Finished) {
				Performer = DBContext.Single.getUserByLogin(tbl.Performer).Name;
				DatePerform = tbl.DatePerform.Value;
			}

		}

		public static TBPComment initTBPComment(TBPInfo tbp) {
			TBPComment com = new TBPComment();
			com.TBPNumber = tbp.Number;
			com.TBPID = tbp.ID;
			if (!string.IsNullOrEmpty(tbp.IDWordData)) {
				BlanksEntities eni=new BlanksEntities();
				DataTable dat=(from d in eni.DataTable where d.ID==tbp.IDWordData select d).FirstOrDefault();
				if (dat!=null){
					com.Data=dat.Data;
					com.FileInfoData="comment_"+dat.FileInfo;
					com.DataID=Guid.NewGuid().ToString();
				}
			}
			return com;
		}

		public static ReturnMessage createComment(TBPComment comment) {
			Logger.info("Добавление замечания к бланку " + comment.TBPNumber);
			BlanksEntities eni = new BlanksEntities();
			try {
				TBPCommentsTable tbl=new TBPCommentsTable();
				tbl.Author=DBContext.Single.GetCurrentUser().Login;
				tbl.Comment=comment.CommentText;
				tbl.DateCreate = DateTime.Now;
				tbl.Finished=false;
				tbl.TBPNumber=comment.TBPNumber;
				tbl.TBPID = comment.TBPID;
				tbl.Id=Guid.NewGuid().ToString();
				if (comment.Data!=null && comment.Data.Length>0){
					Logger.info("Загрузка прикрепленного файла");
					DataTable dat=new DataTable();
					dat.Data=comment.Data;
					dat.ID=Guid.NewGuid().ToString();
					dat.DateCreate = DateTime.Now;
					dat.Author = DBContext.Single.GetCurrentUser().Login;
					tbl.WordData=dat.ID;
					dat.FileInfo = comment.FileInfoData;
					dat.isPDF=false;
					dat.md5 = MD5Class.getString(comment.Data);
					eni.DataTable.Add(dat);
				}
				eni.TBPCommentsTable.Add(tbl);
				eni.SaveChanges();
				MailContext.SendComment(comment);
				return new ReturnMessage(true,"Замечание успешно создано");
			} catch (Exception e) {
				Logger.info("Ошибка при добавлении комментария к записи " + e.ToString());
				return new ReturnMessage(false, "Ошибка при добавлении замечания к бланку");
			}
		}

		public static ReturnMessage finishComment(TBPComment comment) {
			Logger.info("Закрытие замечания к бланку " + comment.TBPNumber);
			BlanksEntities eni = new BlanksEntities();
			try {
				TBPCommentsTable com = (from c in eni.TBPCommentsTable where c.Id == comment.ID select c).FirstOrDefault();
				if (com!=null) {
					com.Performer = DBContext.Single.GetCurrentUser().Login;
					com.DatePerform = DateTime.Now;
					com.Finished = true;
					com.CommentPerform = comment.CommentPerform;
					eni.SaveChanges();
					return new ReturnMessage(true, "Замечание успешно закрыто");
				} else {
					return new ReturnMessage(false, "Замечение не найдено в БД");
				}
			} catch (Exception e) {
				Logger.info("Ошибка при закрытии замечания к бланку " + e.ToString());
				return new ReturnMessage(false, "Ошибка при закрытии замечания к бланку ");
			}
		}
	}
}