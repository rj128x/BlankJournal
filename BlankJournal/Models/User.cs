using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace BlankJournal.Models {



	[DataContract]
	public class User {
		[DataMember]
		public string UserName { get; set; }
		[DataMember]
		public string UserLogin { get; set; }
	}
}