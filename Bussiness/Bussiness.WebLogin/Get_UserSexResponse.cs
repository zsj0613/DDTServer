using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;
namespace Bussiness.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), DebuggerStepThrough, MessageContract(WrapperName = "Get_UserSexResponse", WrapperNamespace = "dandantang", IsWrapped = true)]
	public class Get_UserSexResponse
	{
		[MessageBodyMember(Namespace = "dandantang", Order = 0), XmlElement(IsNullable = true)]
		public bool? Get_UserSexResult;
		public Get_UserSexResponse()
		{
		}
		public Get_UserSexResponse(bool? Get_UserSexResult)
		{
			this.Get_UserSexResult = Get_UserSexResult;
		}
	}
}
