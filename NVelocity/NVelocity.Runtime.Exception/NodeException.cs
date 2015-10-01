using NVelocity.Runtime.Parser.Node;
using System;
using System.Runtime.Serialization;

namespace NVelocity.Runtime.Exception
{
	[Serializable]
	public class NodeException : System.Exception
	{
		public NodeException(string exceptionMessage, INode node) : base(string.Concat(new object[]
		{
			exceptionMessage,
			": ",
			node.Literal,
			" [line ",
			node.Line,
			",column ",
			node.Column,
			"]"
		}))
		{
		}

		public NodeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
