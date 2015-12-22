using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;

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
		public int DoUser(User idr) {
			return 5;
		}
		// Добавьте здесь дополнительные операции и отметьте их атрибутом [OperationContract]
	}
}
