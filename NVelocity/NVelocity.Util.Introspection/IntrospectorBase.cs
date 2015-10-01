using System;
using System.Collections;
using System.Reflection;

namespace NVelocity.Util.Introspection
{
	public abstract class IntrospectorBase
	{
		protected internal Hashtable classMethodMaps = new Hashtable();

		protected internal IList cachedClassNames = new ArrayList();

		public virtual MethodInfo GetMethod(Type c, string name, object[] parameters)
		{
			if (c == null)
			{
				throw new System.Exception("Introspector.getMethod(): Class method key was null: " + name);
			}
			ClassMap classMap = null;
			lock (this.classMethodMaps)
			{
				classMap = (ClassMap)this.classMethodMaps[c];
				if (classMap == null)
				{
					if (this.cachedClassNames.Contains(c.FullName))
					{
						this.ClearCache();
					}
					classMap = this.CreateClassMap(c);
				}
			}
			return classMap.FindMethod(name, parameters);
		}

		public virtual PropertyInfo GetProperty(Type c, string name)
		{
			if (c == null)
			{
				throw new System.Exception("Introspector.getMethod(): Class method key was null: " + name);
			}
			ClassMap classMap = null;
			lock (this.classMethodMaps)
			{
				classMap = (ClassMap)this.classMethodMaps[c];
				if (classMap == null)
				{
					if (this.cachedClassNames.Contains(c.FullName))
					{
						this.ClearCache();
					}
					classMap = this.CreateClassMap(c);
				}
			}
			return classMap.FindProperty(name);
		}

		protected internal ClassMap CreateClassMap(Type c)
		{
			ClassMap classMap = new ClassMap(c);
			this.classMethodMaps[c] = classMap;
			this.cachedClassNames.Add(c.FullName);
			return classMap;
		}

		protected internal virtual void ClearCache()
		{
			this.classMethodMaps.Clear();
			this.cachedClassNames = new ArrayList();
		}
	}
}
