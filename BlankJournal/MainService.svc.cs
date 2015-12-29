using BlankJournal.Models;
using System;
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
		}

		public ReturnMessage(int maxLSO, string lastOBP) {
			MaxLSO = maxLSO;
			LastOBP = lastOBP;
		}
	
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
		public IQueryable<TBPInfo> GetTBPBlanksByFolder(int folderID) {
			return DBContext.Single.GetTBPListByFolder(folderID).AsQueryable();
		}

		[OperationContract]
		public IQueryable<JournalRecord> GetJournalBP() {
			return DBContext.Single.GetJournalBP().AsQueryable();
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
		public IQueryable<TBPComment> GetCommentsList() {
			return DBContext.Single.GetCommentsList().AsQueryable();
		}

		[OperationContract]
		public ReturnMessage getOperationsInfo() {
			return new ReturnMessage(DBContext.Single.MaxLSO, DBContext.Single.LastOBP);
		}

		// Добавьте здесь дополнительные операции и отметьте их атрибутом [OperationContract]
	}
}
