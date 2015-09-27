using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
namespace Game.Server.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), DebuggerStepThrough]
	public class PassPortHttpPostClient : ClientBase<PassPortHttpPost>, PassPortHttpPost
	{
		public PassPortHttpPostClient()
		{
		}
		public PassPortHttpPostClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}
		public PassPortHttpPostClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}
		public PassPortHttpPostClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}
		public PassPortHttpPostClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		ChenckValidateResponse1 PassPortHttpPost.ChenckValidate(ChenckValidateRequest1 request)
		{
			return base.Channel.ChenckValidate(request);
		}
		public string ChenckValidate(string username, string password)
		{
			ChenckValidateResponse1 retVal = ((PassPortHttpPost)this).ChenckValidate(new ChenckValidateRequest1
			{
				username = username,
				password = password
			});
			return retVal.@string;
		}
	}
}
