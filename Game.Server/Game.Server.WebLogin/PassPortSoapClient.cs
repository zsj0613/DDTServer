using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
namespace Game.Server.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), DebuggerStepThrough]
	public class PassPortSoapClient : ClientBase<PassPortSoap>, PassPortSoap
	{
		public PassPortSoapClient()
		{
		}
		public PassPortSoapClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}
		public PassPortSoapClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}
		public PassPortSoapClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}
		public PassPortSoapClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}
		public string ChenckValidate(string username, string password)
		{
			return base.Channel.ChenckValidate(username, password);
		}
	}
}
