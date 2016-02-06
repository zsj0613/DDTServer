using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
namespace Bussiness.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), DebuggerStepThrough, MessageContract(IsWrapped = false)]
	public class Get_UserSexRequest2
	{
		[MessageBodyMember(Namespace = "", Order = 0)]
		public string username;
		public Get_UserSexRequest2()
		{
		}
		public Get_UserSexRequest2(string username)
		{
			this.username = username;
		}
	}
}
