using BlankJournal.Models;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;

namespace BlankJournal {
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


		// Добавьте здесь дополнительные операции и отметьте их атрибутом [OperationContract]
	}
}
