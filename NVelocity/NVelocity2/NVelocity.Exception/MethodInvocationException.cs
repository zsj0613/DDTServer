using System;
using System.Runtime.Serialization;

namespace NVelocity.Exception
{
	[Serializable]
	public class MethodInvocationException : VelocityException
	{
		private string methodName = "";

		private string referenceName = "";

		public string MethodName
		{
			get
			{
				return this.methodName;
			}
		}

		public string ReferenceName
		{
			get
			{
				return this.referenceName;
			}
			set
			{
				this.referenceName = value;
			}
		}

		public MethodInvocationException(string message, System.Exception innerException, string methodName) : base(message, innerException)
		{
			this.methodName = methodName;
		}

		protected MethodInvocationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.methodName = info.GetString("methodName");
			this.referenceName = info.GetString("referenceName");
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("methodName", this.methodName);
			info.AddValue("referenceName", this.referenceName);
			base.GetObjectData(info, context);
		}
	}
}
