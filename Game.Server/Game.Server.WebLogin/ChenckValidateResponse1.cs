using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Serialization;
namespace Game.Server.WebLogin
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0"), DebuggerStepThrough, MessageContract(IsWrapped = false)]
	public class ChenckValidateResponse1
	{
		[MessageBodyMember(Namespace = "dandantang", Order = 0), XmlElement(IsNullable = true)]
		public string @string;
		public ChenckValidateResponse1()
		{
		}
		public ChenckValidateResponse1(string @string)
		{
			this.@string = @string;
		}
	}
}
