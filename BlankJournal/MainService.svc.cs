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
		public Users GetUser() {
			string login = HttpContext.Current.User.Identity.Name;
			Users usr = new Users();
			usr.UserName = login;
			
			return usr;
		}
		// Добавьте здесь дополнительные операции и отметьте их атрибутом [OperationContract]
	}
}
