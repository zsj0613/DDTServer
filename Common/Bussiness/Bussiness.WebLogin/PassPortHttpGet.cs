using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
namespace Bussiness.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), ServiceContract(Namespace = "dandantang", ConfigurationName = "WebLogin.PassPortHttpGet")]
	public interface PassPortHttpGet
	{
		[OperationContract(Action = "dandantang/PassPortHttpGet/ChenckValidateRequest", ReplyAction = "dandantang/PassPortHttpGet/ChenckValidateResponse"), XmlSerializerFormat]
		ChenckValidateResponse ChenckValidate(ChenckValidateRequest request);
		[OperationContract(Action = "dandantang/PassPortHttpGet/Get_UserSexRequest", ReplyAction = "dandantang/PassPortHttpGet/Get_UserSexResponse"), XmlSerializerFormat]
		Get_UserSexResponse1 Get_UserSex(Get_UserSexRequest1 request);
	}
}
