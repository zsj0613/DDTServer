using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
namespace Bussiness.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), ServiceContract(Namespace = "dandantang", ConfigurationName = "WebLogin.PassPortHttpPost")]
	public interface PassPortHttpPost
	{
		[OperationContract(Action = "dandantang/PassPortHttpPost/ChenckValidateRequest", ReplyAction = "dandantang/PassPortHttpPost/ChenckValidateResponse"), XmlSerializerFormat]
		ChenckValidateResponse1 ChenckValidate(ChenckValidateRequest1 request);
		[OperationContract(Action = "dandantang/PassPortHttpPost/Get_UserSexRequest", ReplyAction = "dandantang/PassPortHttpPost/Get_UserSexResponse"), XmlSerializerFormat]
		Get_UserSexResponse2 Get_UserSex(Get_UserSexRequest2 request);
	}
}
