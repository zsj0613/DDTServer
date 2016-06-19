using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace NVelocity.Util.Introspection
{
	public class ClassMap
	{
		private sealed class CacheMiss
		{
		}

		private static readonly ClassMap.CacheMiss CACHE_MISS = new ClassMap.CacheMiss();

		private static readonly object OBJECT = new object();

		private readonly Type clazz;

		private readonly Hashtable methodCache = new Hashtable();

		private readonly Hashtable propertyCache = new Hashtable();

		private readonly MethodMap methodMap = new MethodMap();

		internal Type CachedClass
		{
			get
			{
				return this.clazz;
			}
		}

		public ClassMap(Type clazz)
		{
			this.clazz = clazz;
			this.PopulateMethodCache();
			this.PopulatePropertyCache();
		}

		public ClassMap()
		{
		}

		public MethodInfo FindMethod(string name, object[] params_Renamed)
		{
			string key = ClassMap.MakeMethodKey(name, params_Renamed);
			object obj = this.methodCache[key];
			MethodInfo result;
			if (obj == ClassMap.CACHE_MISS)
			{
				result = null;
			}
			else
			{
				if (obj == null)
				{
					try
					{
						obj = this.methodMap.Find(name, params_Renamed);
					}
					catch (AmbiguousException)
					{
						this.methodCache[key] = ClassMap.CACHE_MISS;
						throw;
					}
					this.methodCache[key] = ((obj == null) ? ClassMap.CACHE_MISS : obj);
				}
				result = (MethodInfo)obj;
			}
			return result;
		}

		public PropertyInfo FindProperty(string name)
		{
			object obj = this.propertyCache[name];
			PropertyInfo result;
			if (obj == ClassMap.CACHE_MISS)
			{
				result = null;
			}
			else
			{
				result = (PropertyInfo)obj;
			}
			return result;
		}

		private void PopulateMethodCache()
		{
			MethodInfo[] accessibleMethods = ClassMap.GetAccessibleMethods(this.clazz);
			MethodInfo[] array = accessibleMethods;
			for (int i = 0; i < array.Length; i++)
			{
				MethodInfo methodInfo = array[i];
				this.methodMap.Add(methodInfo);
				this.methodCache[this.MakeMethodKey(methodInfo)] = methodInfo;
			}
		}

		private void PopulatePropertyCache()
		{
			PropertyInfo[] accessibleProperties = ClassMap.GetAccessibleProperties(this.clazz);
			PropertyInfo[] array = accessibleProperties;
			for (int i = 0; i < array.Length; i++)
			{
				PropertyInfo propertyInfo = array[i];
				this.propertyCache[propertyInfo.Name] = propertyInfo;
			}
		}

		private string MakeMethodKey(MethodInfo method)
		{
			StringBuilder stringBuilder = new StringBuilder(method.Name);
			ParameterInfo[] parameters = method.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				stringBuilder.Append(parameterInfo.ParameterType.FullName);
			}
			return stringBuilder.ToString();
		}

		private static string MakeMethodKey(string method, object[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder(method);
			if (parameters != null)
			{
				for (int i = 0; i < parameters.Length; i++)
				{
					object obj = parameters[i];
					if (obj == null)
					{
						obj = ClassMap.OBJECT;
					}
					stringBuilder.Append(obj.GetType().FullName);
				}
			}
			return stringBuilder.ToString();
		}

		private static MethodInfo[] GetAccessibleMethods(Type clazz)
		{
			ArrayList arrayList = new ArrayList();
			Type[] interfaces = clazz.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				Type type = interfaces[i];
				arrayList.AddRange(type.GetMethods());
			}
			arrayList.AddRange(clazz.GetMethods());
			return (MethodInfo[])arrayList.ToArray(typeof(MethodInfo));
		}

		private static PropertyInfo[] GetAccessibleProperties(Type clazz)
		{
			ArrayList arrayList = new ArrayList();
			Type[] interfaces = clazz.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				Type type = interfaces[i];
				arrayList.AddRange(type.GetProperties());
			}
			arrayList.AddRange(clazz.GetProperties());
			return (PropertyInfo[])arrayList.ToArray(typeof(PropertyInfo));
		}
	}
}
