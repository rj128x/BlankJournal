using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal.Models {
	public class TBPComment {
		public string ID { get; set; }
		public string TBPNumber { get; set; }
		public string Author { get; set; }
		public string CommentText { get; set; }
		public DateTime DateCreate { get; set; }
		public string Performer { get; set; }
		public DateTime DatePerform { get; set; }
		public bool Finished { get; set; }
		public string DataID { get; set; }
		public byte[] Data { get; set; }
		public string CommentPerform { get; set; }

		public TBPComment() { }

		public TBPComment(TBPCommentsTable tbl){
			ID = tbl.Id;
			TBPNumber = tbl.TBPNumber;
			Author = DBContext.Single.getUserByLogin(tbl.Author).Name;
			CommentText = tbl.Comment;
			DateCreate = tbl.DateCreate;
			Finished = tbl.Finished;
			CommentPerform = tbl.CommentPerform;
			if (Finished) {
				Performer = DBContext.Single.getUserByLogin(tbl.Performer).Name;
				DatePerform = tbl.DatePerform.Value;
			}

		}

		public static ReturnMessage createComment(TBPComment comment) {
			BlanksEntities eni = new BlanksEntities();
			try {
				TBPCommentsTable tbl=new TBPCommentsTable();
				tbl.Author=DBContext.Single.GetCurrentUser().Login;
				tbl.Comment=comment.CommentText;
				tbl.DateCreate=DateTime.Now;
				tbl.Finished=false;
				tbl.TBPNumber=comment.TBPNumber;
				tbl.Id=Guid.NewGuid().ToString();
				if (comment.Data!=null && comment.Data.Length>0){
					DataTable dat=new DataTable();
					dat.Data=comment.Data;
					dat.ID=Guid.NewGuid().ToString();
					tbl.WordData=dat.ID;
					dat.isPDF=false;
					eni.DataTable.Add(dat);
				}
				eni.TBPCommentsTable.Add(tbl);
				eni.SaveChanges();
				return new ReturnMessage(true,"Замечание успешно создано");
			} catch (Exception e) {
				Logger.info("Ошибка при добавлении комментария к записи " + e.ToString());
				return new ReturnMessage(false, "Ошибка при добавлении замечания к бланку");
			}
		}

		public static ReturnMessage finishComment(TBPComment comment) {
			BlanksEntities eni = new BlanksEntities();
			try {
				IQueryable<TBPCommentsTable> lst = from c in eni.TBPCommentsTable where c.Id == comment.ID select c;
				if (lst.Count() > 0) {
					TBPCommentsTable last = lst.First();
					last.Performer = DBContext.Single.GetCurrentUser().Login;
					last.DatePerform = DateTime.Now;
					last.Finished = true;
					last.CommentPerform = comment.CommentPerform;
					eni.SaveChanges();
					return new ReturnMessage(true,"Замечание успешно закрыто");
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