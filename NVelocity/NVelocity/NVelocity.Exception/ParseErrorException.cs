using System;
using System.Runtime.Serialization;

namespace NVelocity.Exception
{
	[Serializable]
	public class ParseErrorException : VelocityException
	{
		public ParseErrorException(string exceptionMessage) : base(exceptionMessage)
		{
		}

		public ParseErrorException(string exceptionMessage, System.Exception innerException) : base(exceptionMessage, innerException)
		{
		}

		protected ParseErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
