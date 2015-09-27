using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
namespace Bussiness.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), DebuggerStepThrough, MessageContract(IsWrapped = false)]
	public class Get_UserSexRequest1
	{
		[MessageBodyMember(Namespace = "", Order = 0)]
		public string username;
		public Get_UserSexRequest1()
		{
		}
		public Get_UserSexRequest1(string username)
		{
			this.username = username;
		}
	}
}
