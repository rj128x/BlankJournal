using BlankJournal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;

namespace BlankJournal {

	public class ReturnMessage {
		public string Message { get; set; }
		public bool Result { get; set; }
		public int MaxLSO { get; set; }
		public string LastOBP { get; set; }
		public ReturnMessage() { }
		public ReturnMessage(bool result, string message) {
			Message = message;
			Result = result;
			Logger.info(String.Format("Возврат: {0} (1)", result, message));
		}

		public ReturnMessage(int maxLSO, string lastOBP) {
			MaxLSO = maxLSO;
			LastOBP = lastOBP;
			Logger.info(String.Format("Возврат: LSO:{0} OBP(1)", maxLSO, lastOBP));
		}
	
	}

	public class JournalAnswer {
		public DateTime? dateStart { get; set; }
		public DateTime? dateEnd { get; set; }
		public string tbpNumber { get; set; }
		public bool checkCrossData { get; set; }
		public List<JournalRecord> Data{get;set;}
	}

	public class CommentAnswer {
		public DateTime? dateStart { get; set; }
		public DateTime? dateEnd { get; set; }
		public bool onlyActive { get; set; }
		public List<TBPComment> Data { get; set; }
	}

	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class MainService {
		[OperationContract]
		public void DoWork() {
			// Добавьте здесь реализацию операции
			return;
		}

		[OperationContract]
		public User GetUser() {
			return DBContext.Single.GetCurrentUser();
		}

		[OperationContract]
		public IQueryable<Folder> GetAllFolders() {
			return DBContext.Single.AllFolders.Values.AsQueryable();
		}

		[OperationContract]
		public IQueryable<TBPInfo> GetTBPBlanksByFolder(string folderID) {
			return DBContext.Single.GetTBPListByFolder(folderID).AsQueryable();
		}

		[OperationContract]
		public JournalAnswer GetJournalBP(JournalAnswer Filter) {
			
			return DBContext.Single.GetJournalBP(Filter);
		}

		[OperationContract]
		public ReturnMessage CreateTBP(TBPInfo newBlank) {
			return DBContext.Single.createTBP(newBlank,newBlank.EditingTBP);
		}

		[OperationContract]
		public JournalRecord InitTBP(TBPInfo tbp) {
			return JournalRecord.initTBPRecord(tbp);
		}

		[OperationContract]
		public JournalRecord InitOBP(TBPInfo tbp) {
			return JournalRecord.initOBPRecord(tbp);		
		}

		[OperationContract]
		public TBPComment InitComment(TBPInfo tbp) {
			return TBPComment.initTBPComment(tbp);
		}

		[OperationContract]
		public IQueryable<TBPHistoryRecord> getHistory(TBPInfo tbp) {
			return DBContext.Single.getTBPHistory(tbp).AsQueryable();
		}

		[OperationContract]
		public ReturnMessage CreateBP(JournalRecord journal) {
			return JournalRecord.CreateBP(journal);
		}

		[OperationContract]
		public ReturnMessage FinishBP(JournalRecord journal) {
			return JournalRecord.CreateBP(journal);
		}

		[OperationContract]
		public ReturnMessage CreateCommentTBP(TBPComment comment) {
			return TBPComment.createComment(comment);
		}

		[OperationContract]
		public ReturnMessage FinishCommentTBP(TBPComment comment) {
			return TBPComment.finishComment(comment);
		}

		[OperationContract]
		public CommentAnswer GetCommentsList(CommentAnswer Filter) {
			return DBContext.Single.GetCommentsList(Filter);
		}

		[OperationContract]
		public ReturnMessage getOperationsInfo() {
			return new ReturnMessage(DBContext.Single.MaxLSO, DBContext.Single.LastOBP);
		}

		[OperationContract]
		public IQueryable<User> getAllUsers() {
			return DBContext.Single.AllUsers.Values.AsQueryable();
		}

		[OperationContract]
		public ReturnMessage editUser(User user) {
			return User.EditUser(user);
		}

		[OperationContract]
		public ReturnMessage deleteUser(User user) {
			return User.DeleteUser(user);
		}

		[OperationContract]
		public ReturnMessage addFile(string fileInfo, byte[] data,DateTime dateLoad) {
			return InitDB.processFileData(fileInfo, data,dateLoad);
		}

		[OperationContract]
		public ReturnMessage removeTBP(TBPInfo tbp) {
			return DBContext.Single.removeTBP(tbp);
		}

		[OperationContract]
		public DataRecord getDataRecord(string id) {
			return DataRecord.getDataRecord(id);
		}

		[OperationContract]
		public ReturnMessage updateDataRecord(DataRecord rec) {
			return DataRecord.UpdateFile(rec);
		}

		[OperationContract]
		public void LogInfo(string message, DateTime date) {
			Logger.info(String.Format("Client: {0}: {1}", date.ToString("dd.MM.yyyy hh:mm:ss,fff"), message));
		}

	
		// Добавьте здесь дополнительные операции и отметьте их атрибутом [OperationContract]
	}
}
