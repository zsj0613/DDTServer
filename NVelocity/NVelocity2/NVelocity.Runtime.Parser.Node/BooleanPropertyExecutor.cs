using NVelocity.Util.Introspection;
using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class BooleanPropertyExecutor : PropertyExecutor
	{
		public BooleanPropertyExecutor(IRuntimeLogger r, Introspector i, Type clazz, string propertyName) : base(r, i, clazz, propertyName)
		{
		}

		protected internal override void Discover(Type clazz, string propertyName)
		{
			try
			{
				this.property = this.introspector.GetProperty(clazz, propertyName);
				if (this.property == null || !this.property.PropertyType.Equals(typeof(bool)))
				{
					propertyName = propertyName.Substring(0, 1).ToUpper() + propertyName.Substring(1);
					this.property = this.introspector.GetProperty(clazz, propertyName);
					if (this.property == null || !this.property.PropertyType.Equals(typeof(bool)))
					{
						this.property = null;
					}
				}
			}
			catch (System.Exception arg)
			{
				this.rlog.Error("PROGRAMMER ERROR : BooleanPropertyExector() : " + arg);
			}
		}
	}
}
