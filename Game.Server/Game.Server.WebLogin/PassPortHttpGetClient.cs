using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
namespace Game.Server.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), DebuggerStepThrough]
	public class PassPortHttpGetClient : ClientBase<PassPortHttpGet>, PassPortHttpGet
	{
		public PassPortHttpGetClient()
		{
		}
		public PassPortHttpGetClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}
		public PassPortHttpGetClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}
		public PassPortHttpGetClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}
		public PassPortHttpGetClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		ChenckValidateResponse PassPortHttpGet.ChenckValidate(ChenckValidateRequest request)
		{
			return base.Channel.ChenckValidate(request);
		}
		public string ChenckValidate(string username, string password)
		{
			ChenckValidateResponse retVal = ((PassPortHttpGet)this).ChenckValidate(new ChenckValidateRequest
			{
				username = username,
				password = password
			});
			return retVal.@string;
		}
	}
}
