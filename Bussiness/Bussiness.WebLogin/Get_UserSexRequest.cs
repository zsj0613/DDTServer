using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
namespace Bussiness.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), DebuggerStepThrough, MessageContract(WrapperName = "Get_UserSex", WrapperNamespace = "dandantang", IsWrapped = true)]
	public class Get_UserSexRequest
	{
		[MessageBodyMember(Namespace = "dandantang", Order = 0)]
		public string username;
		public Get_UserSexRequest()
		{
		}
		public Get_UserSexRequest(string username)
		{
			this.username = username;
		}
	}
}
