using System;

namespace NVelocity.App.Events
{
	public class MethodExceptionEventArgs : EventArgs
	{
		private object valueToRender;

		private System.Exception exceptionThrown;

		private Type targetClass;

		private string targetMethod;

		public object ValueToRender
		{
			get
			{
				return this.valueToRender;
			}
			set
			{
				this.valueToRender = value;
			}
		}

		public System.Exception ExceptionThrown
		{
			get
			{
				return this.exceptionThrown;
			}
		}

		public Type TargetClass
		{
			get
			{
				return this.targetClass;
			}
		}

		public string TargetMethod
		{
			get
			{
				return this.targetMethod;
			}
		}

		public MethodExceptionEventArgs(Type targetClass, string targetMethod, System.Exception exceptionThrown)
		{
			this.targetClass = targetClass;
			this.targetMethod = targetMethod;
			this.exceptionThrown = exceptionThrown;
		}
	}
}
