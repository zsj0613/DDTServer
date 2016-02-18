using System;
using System.Runtime.Serialization;

namespace NVelocity.Exception
{
	[Serializable]
	public class VelocityException : System.Exception
	{
		public VelocityException(string exceptionMessage) : base(exceptionMessage)
		{
		}

		public VelocityException(string message, System.Exception innerException) : base(message, innerException)
		{
		}

		protected VelocityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
