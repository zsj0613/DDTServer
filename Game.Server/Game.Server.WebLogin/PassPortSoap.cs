using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
namespace Game.Server.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), ServiceContract(Namespace = "dandantang", ConfigurationName = "WebLogin.PassPortSoap")]
	public interface PassPortSoap
	{
		[OperationContract(Action = "dandantang/ChenckValidate", ReplyAction = "*"), XmlSerializerFormat]
		string ChenckValidate(string username, string password);
	}
}
