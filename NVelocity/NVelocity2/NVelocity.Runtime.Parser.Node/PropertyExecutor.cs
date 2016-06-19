using NVelocity.Util.Introspection;
using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class PropertyExecutor : AbstractExecutor
	{
		private string propertyUsed = null;

		protected Introspector introspector = null;

		public PropertyExecutor(IRuntimeLogger r, Introspector i, Type clazz, string propertyName)
		{
			this.rlog = r;
			this.introspector = i;
			this.Discover(clazz, propertyName);
		}

		protected internal virtual void Discover(Type clazz, string propertyName)
		{
			try
			{
				this.propertyUsed = propertyName;
				this.property = this.introspector.GetProperty(clazz, this.propertyUsed);
				if (this.property == null)
				{
					this.propertyUsed = propertyName.Substring(0, 1).ToUpper() + propertyName.Substring(1);
					this.property = this.introspector.GetProperty(clazz, this.propertyUsed);
					if (this.property == null)
					{
						this.propertyUsed = propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
						this.property = this.introspector.GetProperty(clazz, this.propertyUsed);
						if (this.property == null)
						{
							this.propertyUsed = propertyName;
							this.method = this.introspector.GetMethod(clazz, this.propertyUsed, new object[0]);
							if (this.method == null)
							{
								this.propertyUsed = propertyName.Substring(0, 1).ToUpper() + propertyName.Substring(1);
								this.method = this.introspector.GetMethod(clazz, this.propertyUsed, new object[0]);
								if (this.method == null)
								{
									this.propertyUsed = propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
									this.method = this.introspector.GetMethod(clazz, this.propertyUsed, new object[0]);
									if (this.method != null)
									{
									}
								}
							}
						}
					}
				}
			}
			catch (System.Exception arg)
			{
				this.rlog.Error("PROGRAMMER ERROR : PropertyExector() : " + arg);
			}
		}

		public override object Execute(object o)
		{
			object result;
			if (this.property == null && this.method == null)
			{
				result = null;
			}
			else if (this.property != null)
			{
				result = this.property.GetValue(o, null);
			}
			else
			{
				result = this.method.Invoke(o, new object[0]);
			}
			return result;
		}
	}
}
