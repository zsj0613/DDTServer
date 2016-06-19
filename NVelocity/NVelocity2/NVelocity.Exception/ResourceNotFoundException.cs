using System;
using System.Runtime.Serialization;

namespace NVelocity.Exception
{
	[Serializable]
	public class ResourceNotFoundException : VelocityException
	{
		public ResourceNotFoundException(string exceptionMessage) : base(exceptionMessage)
		{
		}

		public ResourceNotFoundException(string message, System.Exception innerException) : base(message, innerException)
		{
		}

		public ResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
