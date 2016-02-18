using NVelocity.Context;
using NVelocity.Exception;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace NVelocity.App.Events
{
	public class EventCartridge
	{
        public event ReferenceInsertionEventHandler ReferenceInsertion;

        public event NullSetEventHandler NullSet;

        public event MethodExceptionEventHandler MethodExceptionEvent;

		internal object ReferenceInsert(Stack referenceStack, string reference, object value)
		{
			if (this.ReferenceInsertion != null)
			{
				ReferenceInsertionEventArgs referenceInsertionEventArgs = new ReferenceInsertionEventArgs(referenceStack, reference, value);
				this.ReferenceInsertion(this, referenceInsertionEventArgs);
				value = referenceInsertionEventArgs.NewValue;
			}
			return value;
		}

		internal bool ShouldLogOnNullSet(string lhs, string rhs)
		{
			bool result;
			if (this.NullSet == null)
			{
				result = true;
			}
			else
			{
				NullSetEventArgs nullSetEventArgs = new NullSetEventArgs(lhs, rhs);
				this.NullSet(this, nullSetEventArgs);
				result = nullSetEventArgs.ShouldLog;
			}
			return result;
		}

		internal object HandleMethodException(Type claz, string method, System.Exception e)
		{
			if (this.MethodExceptionEvent == null)
			{
				throw new VelocityException(e.Message, e);
			}
			MethodExceptionEventArgs methodExceptionEventArgs = new MethodExceptionEventArgs(claz, method, e);
			this.MethodExceptionEvent(this, methodExceptionEventArgs);
			if (methodExceptionEventArgs.ValueToRender != null)
			{
				return methodExceptionEventArgs.ValueToRender;
			}
			throw new VelocityException(e.Message, e);
		}

		public bool AttachToContext(IContext context)
		{
			bool result;
			if (context is IInternalEventContext)
			{
				IInternalEventContext internalEventContext = (IInternalEventContext)context;
				internalEventContext.AttachEventCartridge(this);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
