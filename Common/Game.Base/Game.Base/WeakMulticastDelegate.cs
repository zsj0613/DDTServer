
using Lsj.Util.Logs;
using System;
using System.Reflection;
using System.Text;
namespace Game.Base
{
	public class WeakMulticastDelegate
	{

		private WeakReference weakRef = null;
		private MethodInfo method = null;
		private WeakMulticastDelegate prev = null;
		public WeakMulticastDelegate(Delegate realDelegate)
		{
			if (realDelegate.Target != null)
			{
				this.weakRef = new WeakRef(realDelegate.Target);
			}
			this.method = realDelegate.Method;
		}
		public static WeakMulticastDelegate Combine(WeakMulticastDelegate weakDelegate, Delegate realDelegate)
		{
			WeakMulticastDelegate result;
			if (realDelegate == null)
			{
				result = null;
			}
			else
			{
				result = ((weakDelegate == null) ? new WeakMulticastDelegate(realDelegate) : weakDelegate.Combine(realDelegate));
			}
			return result;
		}
		public static WeakMulticastDelegate CombineUnique(WeakMulticastDelegate weakDelegate, Delegate realDelegate)
		{
			WeakMulticastDelegate result;
			if (realDelegate == null)
			{
				result = null;
			}
			else
			{
				result = ((weakDelegate == null) ? new WeakMulticastDelegate(realDelegate) : weakDelegate.CombineUnique(realDelegate));
			}
			return result;
		}
		private WeakMulticastDelegate Combine(Delegate realDelegate)
		{
			this.prev = new WeakMulticastDelegate(realDelegate)
			{
				prev = this.prev
			};
			return this;
		}
		protected bool Equals(Delegate realDelegate)
		{
			bool result;
			if (this.weakRef == null)
			{
				result = (realDelegate.Target == null && this.method == realDelegate.Method);
			}
			else
			{
				result = (this.weakRef.Target == realDelegate.Target && this.method == realDelegate.Method);
			}
			return result;
		}
		private WeakMulticastDelegate CombineUnique(Delegate realDelegate)
		{
			bool found = this.Equals(realDelegate);
			if (!found && this.prev != null)
			{
				WeakMulticastDelegate curNode = this.prev;
				while (!found && curNode != null)
				{
					if (curNode.Equals(realDelegate))
					{
						found = true;
					}
					curNode = curNode.prev;
				}
			}
			return found ? this : this.Combine(realDelegate);
		}
		public static WeakMulticastDelegate operator +(WeakMulticastDelegate d, Delegate realD)
		{
			return WeakMulticastDelegate.Combine(d, realD);
		}
		public static WeakMulticastDelegate operator -(WeakMulticastDelegate d, Delegate realD)
		{
			return WeakMulticastDelegate.Remove(d, realD);
		}
		public static WeakMulticastDelegate Remove(WeakMulticastDelegate weakDelegate, Delegate realDelegate)
		{
			WeakMulticastDelegate result;
			if (realDelegate == null || weakDelegate == null)
			{
				result = null;
			}
			else
			{
				result = weakDelegate.Remove(realDelegate);
			}
			return result;
		}
		private WeakMulticastDelegate Remove(Delegate realDelegate)
		{
			WeakMulticastDelegate result;
			if (this.Equals(realDelegate))
			{
				result = this.prev;
			}
			else
			{
				WeakMulticastDelegate current = this.prev;
				WeakMulticastDelegate last = this;
				while (current != null)
				{
					if (current.Equals(realDelegate))
					{
						last.prev = current.prev;
						current.prev = null;
						break;
					}
					last = current;
					current = current.prev;
				}
				result = this;
			}
			return result;
		}
		public void Invoke(object[] args)
		{
			for (WeakMulticastDelegate current = this; current != null; current = current.prev)
			{
				int start = Environment.TickCount;
				if (current.weakRef == null)
				{
					current.method.Invoke(null, args);
				}
				else
				{
					if (current.weakRef.IsAlive)
					{
						current.method.Invoke(current.weakRef.Target, args);
					}
				}
				if (Environment.TickCount - start > 500)
				{
					LogProvider.Default.Warn(string.Concat(new object[]
						{
							"Invoke took ",
							Environment.TickCount - start,
							"ms! ",
							current.ToString()
						}));
				}
			}
		}
		public void InvokeSafe(object[] args)
		{
			for (WeakMulticastDelegate current = this; current != null; current = current.prev)
			{
				int start = Environment.TickCount;
				try
				{
					if (current.weakRef == null)
					{
						current.method.Invoke(null, args);
					}
					else
					{
						if (current.weakRef.IsAlive)
						{
							current.method.Invoke(current.weakRef.Target, args);
						}
					}
				}
				catch (Exception ex)
				{
                    LogProvider.Default.Error("InvokeSafe", ex);
				}
				if (Environment.TickCount - start > 500)
				{
                    LogProvider.Default.Warn(string.Concat(new object[]
						{
							"InvokeSafe took ",
							Environment.TickCount - start,
							"ms! ",
							current.ToString()
						}));
				}
			}
		}
		public string Dump()
		{
			StringBuilder builder = new StringBuilder();
			WeakMulticastDelegate current = this;
			int count = 0;
			while (current != null)
			{
				count++;
				if (current.weakRef == null)
				{
					builder.Append("\t");
					builder.Append(count);
					builder.Append(") ");
					builder.Append(current.method.Name);
					builder.Append(Environment.NewLine);
				}
				else
				{
					if (current.weakRef.IsAlive)
					{
						builder.Append("\t");
						builder.Append(count);
						builder.Append(") ");
						builder.Append(current.weakRef.Target);
						builder.Append(".");
						builder.Append(current.method.Name);
						builder.Append(Environment.NewLine);
					}
					else
					{
						builder.Append("\t");
						builder.Append(count);
						builder.Append(") INVALID.");
						builder.Append(current.method.Name);
						builder.Append(Environment.NewLine);
					}
				}
				current = current.prev;
			}
			return builder.ToString();
		}
		public override string ToString()
		{
			Type declaringType = null;
			if (this.method != null)
			{
				declaringType = this.method.DeclaringType;
			}
			object target = null;
			if (this.weakRef != null && this.weakRef.IsAlive)
			{
				target = this.weakRef.Target;
			}
			return new StringBuilder(64).Append("method: ").Append((declaringType == null) ? "(null)" : declaringType.FullName).Append('.').Append((this.method == null) ? "(null)" : this.method.Name).Append(" target: ").Append((target == null) ? "null" : target.ToString()).ToString();
		}
	}
}
