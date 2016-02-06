using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
namespace Game.Server.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), ServiceContract(Namespace = "dandantang", ConfigurationName = "WebLogin.PassPortHttpPost")]
	public interface PassPortHttpPost
	{
		[OperationContract(Action = "dandantang/PassPortHttpPost/ChenckValidateRequest", ReplyAction = "dandantang/PassPortHttpPost/ChenckValidateResponse"), XmlSerializerFormat]
		ChenckValidateResponse1 ChenckValidate(ChenckValidateRequest1 request);
	}
}
