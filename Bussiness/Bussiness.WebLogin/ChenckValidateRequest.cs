using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
namespace Bussiness.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), DebuggerStepThrough, MessageContract(IsWrapped = false)]
	public class ChenckValidateRequest
	{
		[MessageBodyMember(Namespace = "", Order = 0)]
		public string username;
		[MessageBodyMember(Namespace = "", Order = 1)]
		public string password;
		public ChenckValidateRequest()
		{
		}
		public ChenckValidateRequest(string username, string password)
		{
			this.username = username;
			this.password = password;
		}
	}
}
