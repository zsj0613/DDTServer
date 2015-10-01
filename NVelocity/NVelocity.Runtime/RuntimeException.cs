using System;
using System.Runtime.Serialization;

namespace NVelocity.Runtime
{
	[Serializable]
	public class RuntimeException : System.Exception
	{
		public RuntimeException(string message) : base(message)
		{
		}

		public RuntimeException(string message, System.Exception innerException) : base(message, innerException)
		{
		}

		public RuntimeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
