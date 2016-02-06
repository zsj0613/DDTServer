using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
namespace Bussiness.WebLogin
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
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Get_UserSexResponse PassPortSoap.Get_UserSex(Get_UserSexRequest request)
		{
			return base.Channel.Get_UserSex(request);
		}
		public bool? Get_UserSex(string username)
		{
			Get_UserSexResponse retVal = ((PassPortSoap)this).Get_UserSex(new Get_UserSexRequest
			{
				username = username
			});
			return retVal.Get_UserSexResult;
		}
	}
}
