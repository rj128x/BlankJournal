using BlankJournal.Logging;
using BlankJournal.Models;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace BlankJournal.Services {
	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class AuthorizationService {
		[OperationContract]
		public void DoWork() {

			Logger.info("DOWork",Logger.LoggerSource.service);
		}

		
		


		// Добавьте здесь дополнительные операции и отметьте их атрибутом [OperationContract]
	}
}
