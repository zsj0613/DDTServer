using NVelocity.Runtime;
using System;
using System.Reflection;

namespace NVelocity.Util.Introspection
{
	public class Introspector : IntrospectorBase
	{
		public const string CACHEDUMP_MSG = "Introspector : detected classloader change. Dumping cache.";

		private IRuntimeLogger rlog = null;

		public Introspector(IRuntimeLogger r)
		{
			this.rlog = r;
		}

		public override MethodInfo GetMethod(Type c, string name, object[] parameters)
		{
			MethodInfo result;
			try
			{
				result = base.GetMethod(c, name, parameters);
				return result;
			}
			catch (AmbiguousException)
			{
				string text = "Introspection Error : Ambiguous method invocation " + name + "( ";
				for (int i = 0; i < parameters.Length; i++)
				{
					if (i > 0)
					{
						text += ", ";
					}
					text += parameters[i].GetType().FullName;
				}
				text = text + ") for class " + c;
				this.rlog.Error(text);
			}
			result = null;
			return result;
		}

		public override PropertyInfo GetProperty(Type c, string name)
		{
			PropertyInfo result;
			try
			{
				result = base.GetProperty(c, name);
				return result;
			}
			catch (AmbiguousException)
			{
				string message = string.Concat(new object[]
				{
					"Introspection Error : Ambiguous property invocation ",
					name,
					" for class ",
					c
				});
				this.rlog.Error(message);
			}
			result = null;
			return result;
		}

		protected internal override void ClearCache()
		{
			base.ClearCache();
			this.rlog.Info("Introspector : detected classloader change. Dumping cache.");
		}
	}
}
