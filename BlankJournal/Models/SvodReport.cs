using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlankJournal.Models
{

	public class SvodReport
	{
		public class SvodRecordRow
		{
			public int Sum { get; set; }
			public Dictionary<int, int> MonthInfo;
			public SvodRecordRow() {
				MonthInfo = new Dictionary<int, int>();
				for (int month = 1; month <= 12; month++) {
					MonthInfo[month] = 0;
				}
			}
			public static SvodRecordRow getSumRecord(List<SvodRecordRow> rows) {
				SvodRecordRow sumRow = new SvodRecordRow();
				foreach (SvodRecordRow row in rows) {
					sumRow.Sum += row.Sum;
					for (int month = 1; month <= 12; month++) {
						sumRow.MonthInfo[month] += row.MonthInfo[month];
					}
				}
				return sumRow;
			}
		}

		public int Year { get; set; }
		public Dictionary<string, SvodRecordRow> TBPFoldersInfo;
		public Dictionary<string, SvodRecordRow> OBPFoldersInfo;
		public Dictionary<string, SvodRecordRow> CommentFoldersInfo;
		public Dictionary<string, SvodRecordRow> CommentActiveFoldersInfo;
		public Dictionary<string, SvodRecordRow> CommentUstrFoldersInfo;
		public SvodRecordRow SumInfo;
		public SvodRecordRow TBPInfo;
		public SvodRecordRow OBPInfo;
		public SvodRecordRow OBPEmptyInfo;
		public SvodRecordRow CommentActiveInfo;
		public SvodRecordRow CommentUstrInfo;
		public SvodRecordRow CommentInfo;

		public void ReadData(int year) {
			this.Year = year;
			DateTime DateStart = new DateTime(year, 1, 1);
			DateTime DateEnd = new DateTime(year + 1, 1, 1);
			TBPFoldersInfo = new Dictionary<string, SvodRecordRow>();
			OBPFoldersInfo = new Dictionary<string, SvodRecordRow>();
			CommentFoldersInfo = new Dictionary<string, SvodRecordRow>();
			CommentActiveFoldersInfo = new Dictionary<string, SvodRecordRow>();
			CommentUstrFoldersInfo = new Dictionary<string, SvodRecordRow>();

			foreach (Folder folder in DBContext.Single.AllFolders.Values) {
				if (folder.ID == "del")
					continue;
				TBPFoldersInfo.Add(folder.ID, new SvodRecordRow());
				OBPFoldersInfo.Add(folder.ID, new SvodRecordRow());
				CommentFoldersInfo.Add(folder.ID, new SvodRecordRow());
				CommentActiveFoldersInfo.Add(folder.ID, new SvodRecordRow());
				CommentUstrFoldersInfo.Add(folder.ID, new SvodRecordRow());
			}

			BlankJournal.BlanksEntities eni = new BlanksEntities();
			var blanks = from b in eni.BPJournalTable
									 from tbp in eni.TBPInfoTable.Where(tbp => b.TBPID == tbp.ID)
									 where (b.DateCreate >= DateStart && b.DateCreate <= DateEnd)
									 orderby b.DateCreate
									 select new {
										 blank = b,
										 folderID = tbp.Folder
									 };
			foreach (var blank in blanks) {
				string folder = blank.folderID;
				int month = blank.blank.DateCreate.Month;
				if (blank.blank.isOBP) {
					OBPFoldersInfo[folder].Sum++;
					OBPFoldersInfo[folder].MonthInfo[month]++;
				} else {
					TBPFoldersInfo[folder].Sum++;
					TBPFoldersInfo[folder].MonthInfo[month]++;
				}
			}

			OBPEmptyInfo = new SvodRecordRow();
			var blankEmpty = from b in eni.BPJournalTable
							 where (b.DateCreate >= DateStart && b.DateCreate <= DateEnd && b.TBPID<=0 && b.isOBP)
							 orderby b.DateCreate
							 select b;

			foreach (var be in blankEmpty) {
				int month = be.DateCreate.Month;
				OBPEmptyInfo.Sum++;
				OBPEmptyInfo.MonthInfo[month]++;
			}


			TBPInfo = SvodRecordRow.getSumRecord(TBPFoldersInfo.Values.ToList());
			List<SvodRecordRow> obps = OBPFoldersInfo.Values.ToList();obps.Add(OBPEmptyInfo);
			OBPInfo = SvodRecordRow.getSumRecord(obps);
			List<SvodRecordRow> all = new List<SvodRecordRow>();
			all.Add(TBPInfo); all.Add(OBPInfo);
			SumInfo = SvodRecordRow.getSumRecord(all);

			var commentsCreate = from c in eni.TBPCommentsTable
													 from tbp in eni.TBPInfoTable.Where(tbp => c.TBPID == tbp.ID)
													 where (c.DateCreate <= DateEnd)
													 select new {
														 comment = c,
														 folderID = tbp.Folder
													 };

			foreach (var comment in commentsCreate) {
				string folder = comment.folderID;
				int monthCreate = comment.comment.DateCreate.Month;
				if (comment.comment.DateCreate >= DateStart && comment.comment.DateCreate <= DateEnd) {
					CommentFoldersInfo[folder].Sum++;
					CommentFoldersInfo[folder].MonthInfo[monthCreate]++;
				}

				if (comment.comment.Finished) {
					if (comment.comment.DatePerform.Value.Year == year) {
						int m = comment.comment.DatePerform.Value.Month;
						CommentUstrFoldersInfo[folder].Sum++;
						CommentUstrFoldersInfo[folder].MonthInfo[m]++;
					}
				}

				DateTime df = comment.comment.Finished ? comment.comment.DatePerform.Value.Date.AddDays(1) : DateEnd.Date;
				DateTime d = comment.comment.DateCreate.Date;
				int curM = d.Month;

				while (d <= df) {
					if (d.Month != curM) {
						if (d > DateStart && d <= DateEnd) {
							CommentActiveFoldersInfo[folder].Sum++;
							CommentActiveFoldersInfo[folder].MonthInfo[curM]++;
						}
						curM = d.Month;
					}
					d = d.AddDays(1);
				}

			}


			CommentInfo = SvodRecordRow.getSumRecord(CommentFoldersInfo.Values.ToList());
			CommentActiveInfo = SvodRecordRow.getSumRecord(CommentActiveFoldersInfo.Values.ToList());
			CommentUstrInfo = SvodRecordRow.getSumRecord(CommentUstrFoldersInfo.Values.ToList());
			CommentActiveInfo.Sum = CommentActiveInfo.MonthInfo.Last().Value;
		}


	}
}