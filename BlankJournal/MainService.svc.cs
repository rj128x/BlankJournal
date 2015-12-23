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
		public UsersTable GetUser() {
			return DBContext.Single.GetCurrentUser();
		}
		// Добавьте здесь дополнительные операции и отметьте их атрибутом [OperationContract]
	}
}
