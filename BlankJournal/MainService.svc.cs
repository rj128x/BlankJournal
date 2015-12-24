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
		public ReturnMessage CreateTBP(TBPInfo newBlank) {
			return DBContext.Single.createTBP(newBlank);
		}


		// Добавьте здесь дополнительные операции и отметьте их атрибутом [OperationContract]
	}
}
