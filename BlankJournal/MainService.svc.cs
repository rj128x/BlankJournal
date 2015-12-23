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
			string login = HttpContext.Current.User.Identity.Name;
			UsersTable usr = new UsersTable();
			usr.Name = login;
			
			return usr;
		}
		// Добавьте здесь дополнительные операции и отметьте их атрибутом [OperationContract]
	}
}
