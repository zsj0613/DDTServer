using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
namespace Game.Server.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), ServiceContract(Namespace = "dandantang", ConfigurationName = "WebLogin.PassPortHttpGet")]
	public interface PassPortHttpGet
	{
		[OperationContract(Action = "dandantang/PassPortHttpGet/ChenckValidateRequest", ReplyAction = "dandantang/PassPortHttpGet/ChenckValidateResponse"), XmlSerializerFormat]
		ChenckValidateResponse ChenckValidate(ChenckValidateRequest request);
	}
}
