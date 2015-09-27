using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
namespace Bussiness.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), ServiceContract(Namespace = "dandantang", ConfigurationName = "WebLogin.PassPortSoap")]
	public interface PassPortSoap
	{
		[OperationContract(Action = "dandantang/ChenckValidate", ReplyAction = "*"), XmlSerializerFormat]
		string ChenckValidate(string username, string password);
		[OperationContract(Action = "dandantang/Get_UserSex", ReplyAction = "*"), XmlSerializerFormat]
		Get_UserSexResponse Get_UserSex(Get_UserSexRequest request);
	}
}
