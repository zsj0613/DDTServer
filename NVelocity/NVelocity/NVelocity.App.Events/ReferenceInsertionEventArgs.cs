using System;
using System.Collections;

namespace NVelocity.App.Events
{
	public class ReferenceInsertionEventArgs : EventArgs
	{
		private Stack referenceStack;

		private object originalValue;

		private object newValue;

		private string rootString;

		public string RootString
		{
			get
			{
				return this.rootString;
			}
		}

		public object OriginalValue
		{
			get
			{
				return this.originalValue;
			}
		}

		public object NewValue
		{
			get
			{
				return this.newValue;
			}
			set
			{
				this.newValue = value;
			}
		}

		public ReferenceInsertionEventArgs(Stack referenceStack, string rootString, object value)
		{
			this.rootString = rootString;
			this.referenceStack = referenceStack;
			this.newValue = value;
			this.originalValue = value;
		}

		public Stack GetCopyOfReferenceStack()
		{
			return (Stack)this.referenceStack.Clone();
		}
	}
}
