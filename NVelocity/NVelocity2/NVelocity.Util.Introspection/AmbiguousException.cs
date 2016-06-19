using System;
using System.Runtime.Serialization;

namespace NVelocity.Util.Introspection
{
	[Serializable]
	public class AmbiguousException : System.Exception
	{
		public AmbiguousException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
