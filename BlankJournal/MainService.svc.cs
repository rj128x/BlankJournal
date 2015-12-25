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
		public ReturnMessage() { }
		public ReturnMessage(bool result, string message) {
			Message = message;
			Result = result;
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
		public ReturnMessage CreateBP(JournalRecord journal) {
			return JournalRecord.CreateBP(journal);
		}

		// Добавьте здесь дополнительные операции и отметьте их атрибутом [OperationContract]
	}
}
