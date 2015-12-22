using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace BlankJournal {
	[DataContract]
	public class User {
		[DataMember]
		public string UserName;
		[DataMember]
		public string UserLogin;
	}
}