using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace BlankJournal {
	[ServiceContract(Namespace = "BlankJournal")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class AuthorizationService {
		[OperationContract]
		public void DoWork() {

			Logger.info("DOWork",Logger.LoggerSource.service);
		}

		[OperationContract]
		public User getUser() {
			User user = new User();
			user.UserLogin = "1";
			user.UserName = "masha;";
			return user;
		}

		
		


		// Добавьте здесь дополнительные операции и отметьте их атрибутом [OperationContract]
	}
}
