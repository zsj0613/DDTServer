using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;
namespace Bussiness.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), DebuggerStepThrough, MessageContract(IsWrapped = false)]
	public class Get_UserSexResponse2
	{
		[MessageBodyMember(Namespace = "dandantang", Order = 0), XmlElement(IsNullable = true)]
		public bool? boolean;
		public Get_UserSexResponse2()
		{
		}
		public Get_UserSexResponse2(bool? boolean)
		{
			this.boolean = boolean;
		}
	}
}
