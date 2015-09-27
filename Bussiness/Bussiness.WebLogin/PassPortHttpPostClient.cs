using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
namespace Bussiness.WebLogin
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
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Get_UserSexResponse2 PassPortHttpPost.Get_UserSex(Get_UserSexRequest2 request)
		{
			return base.Channel.Get_UserSex(request);
		}
		public bool? Get_UserSex(string username)
		{
			Get_UserSexResponse2 retVal = ((PassPortHttpPost)this).Get_UserSex(new Get_UserSexRequest2
			{
				username = username
			});
			return retVal.boolean;
		}
	}
}
