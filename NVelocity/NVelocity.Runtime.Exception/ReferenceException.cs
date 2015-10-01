using NVelocity.Runtime.Parser.Node;
using System;
using System.Runtime.Serialization;

namespace NVelocity.Runtime.Exception
{
	[Serializable]
	public class ReferenceException : System.Exception
	{
		public ReferenceException(string exceptionMessage, INode node) : base(string.Concat(new object[]
		{
			exceptionMessage,
			" [line ",
			node.Line,
			",column ",
			node.Column,
			"] : ",
			node.Literal,
			" is not a valid reference."
		}))
		{
		}

		public ReferenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
