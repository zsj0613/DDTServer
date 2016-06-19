using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace NVelocity.Util.Introspection
{
	public class MethodMap
	{
		[Serializable]
		public class AmbiguousException : System.Exception
		{
			public AmbiguousException()
			{
			}

			public AmbiguousException(string message) : base(message)
			{
			}

			public AmbiguousException(string message, System.Exception innerException) : base(message, innerException)
			{
			}

			public AmbiguousException(SerializationInfo info, StreamingContext context) : base(info, context)
			{
			}
		}

		private const int MORE_SPECIFIC = 0;

		private const int LESS_SPECIFIC = 1;

		private const int INCOMPARABLE = 2;

		internal IDictionary methodByNameMap = new Hashtable();

		public void Add(MethodInfo method)
		{
			string name = method.Name;
			IList list = this.Get(name);
			if (list == null)
			{
				list = new ArrayList();
				this.methodByNameMap.Add(name, list);
			}
			list.Add(method);
		}

		public IList Get(string key)
		{
			return (IList)this.methodByNameMap[key];
		}

		public MethodInfo Find(string methodName, object[] args)
		{
			IList list = this.Get(methodName);
			MethodInfo result;
			if (list == null)
			{
				result = null;
			}
			else
			{
				int num = args.Length;
				Type[] array = new Type[num];
				for (int i = 0; i < num; i++)
				{
					object obj = args[i];
					array[i] = ((obj == null) ? null : obj.GetType());
				}
				result = MethodMap.GetMostSpecific(list, array);
			}
			return result;
		}

		private static MethodInfo GetMostSpecific(IList methods, Type[] classes)
		{
			ArrayList applicables = MethodMap.GetApplicables(methods, classes);
			MethodInfo result;
			if (applicables.Count == 0)
			{
				result = null;
			}
			else if (applicables.Count == 1)
			{
				result = (MethodInfo)applicables[0];
			}
			else
			{
				ArrayList arrayList = new ArrayList();
				foreach (MethodInfo methodInfo in applicables)
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					bool flag = false;
					foreach (MethodInfo methodInfo2 in arrayList)
					{
						switch (MethodMap.IsMoreSpecific(parameters, methodInfo2.GetParameters()))
						{
						case 0:
							arrayList.Remove(methodInfo2);
							break;
						case 1:
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						arrayList.Add(methodInfo);
					}
				}
				if (arrayList.Count > 1)
				{
					ArrayList arrayList2 = new ArrayList();
					foreach (MethodInfo methodInfo3 in arrayList)
					{
						if (!methodInfo3.DeclaringType.IsInterface)
						{
							arrayList2.Add(methodInfo3);
						}
					}
					arrayList = arrayList2;
				}
				if (arrayList.Count > 1)
				{
					throw new MethodMap.AmbiguousException(MethodMap.CreateDescriptiveAmbiguousErrorMessage(arrayList, classes));
				}
				result = (MethodInfo)arrayList[0];
			}
			return result;
		}

		private static int IsMoreSpecific(ParameterInfo[] c1, ParameterInfo[] c2)
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < c1.Length; i++)
			{
				if (c1[i] != c2[i])
				{
					flag = (flag || MethodMap.IsStrictMethodInvocationConvertible(c2[i], c1[i]));
					flag2 = (flag2 || MethodMap.IsStrictMethodInvocationConvertible(c1[i], c2[i]));
				}
			}
			int result;
			if (flag)
			{
				if (flag2)
				{
					result = 2;
				}
				else
				{
					result = 0;
				}
			}
			else if (flag2)
			{
				result = 1;
			}
			else
			{
				result = 2;
			}
			return result;
		}

		private static ArrayList GetApplicables(IList methods, Type[] classes)
		{
			ArrayList arrayList = new ArrayList();
			foreach (MethodInfo methodInfo in methods)
			{
				if (MethodMap.IsApplicable(methodInfo, classes))
				{
					arrayList.Add(methodInfo);
				}
			}
			return arrayList;
		}

		private static bool IsApplicable(MethodInfo method, Type[] classes)
		{
			ParameterInfo[] parameters = method.GetParameters();
			int num = 2147483647;
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				if (parameterInfo.IsDefined(typeof(ParamArrayAttribute), false))
				{
					num = i;
					break;
				}
			}
			bool result;
			if (num == 2147483647 && parameters.Length != classes.Length)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < classes.Length; i++)
				{
					ParameterInfo parameterInfo;
					if (i < num)
					{
						parameterInfo = parameters[i];
					}
					else
					{
						parameterInfo = parameters[num];
					}
					if (!MethodMap.IsMethodInvocationConvertible(parameterInfo, classes[i]))
					{
						result = false;
						return result;
					}
				}
				result = true;
			}
			return result;
		}

		private static bool IsMethodInvocationConvertible(ParameterInfo formal, Type actual)
		{
			Type type = formal.ParameterType;
			if (formal.IsDefined(typeof(ParamArrayAttribute), false))
			{
				type = formal.ParameterType.GetElementType();
			}
			bool result;
			if (actual == null && !type.IsPrimitive)
			{
				result = true;
			}
			else if (actual != null && type.IsAssignableFrom(actual))
			{
				result = true;
			}
			else
			{
				if (type.IsPrimitive)
				{
					if (type == typeof(bool) && actual == typeof(bool))
					{
						result = true;
						return result;
					}
					if (type == typeof(char) && actual == typeof(char))
					{
						result = true;
						return result;
					}
					if (type == typeof(byte) && actual == typeof(byte))
					{
						result = true;
						return result;
					}
					if (type == typeof(short) && (actual == typeof(short) || actual == typeof(byte)))
					{
						result = true;
						return result;
					}
					if (type == typeof(int) && (actual == typeof(int) || actual == typeof(short) || actual == typeof(byte)))
					{
						result = true;
						return result;
					}
					if (type == typeof(long) && (actual == typeof(long) || actual == typeof(int) || actual == typeof(short) || actual == typeof(byte)))
					{
						result = true;
						return result;
					}
					if (type == typeof(float) && (actual == typeof(float) || actual == typeof(long) || actual == typeof(int) || actual == typeof(short) || actual == typeof(byte)))
					{
						result = true;
						return result;
					}
					if (type == typeof(double) && (actual == typeof(double) || actual == typeof(float) || actual == typeof(long) || actual == typeof(int) || actual == typeof(short) || actual == typeof(byte)))
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		private static bool IsStrictMethodInvocationConvertible(ParameterInfo formal, ParameterInfo actual)
		{
			bool result;
			if (actual == null && !formal.ParameterType.IsPrimitive)
			{
				result = true;
			}
			else if (formal.ParameterType.IsAssignableFrom(actual.ParameterType))
			{
				result = true;
			}
			else
			{
				if (formal.ParameterType.IsPrimitive)
				{
					if (formal.ParameterType == typeof(short) && actual.ParameterType == typeof(byte))
					{
						result = true;
						return result;
					}
					if (formal.ParameterType == typeof(int) && (actual.ParameterType == typeof(short) || actual.ParameterType == typeof(byte)))
					{
						result = true;
						return result;
					}
					if (formal.ParameterType == typeof(long) && (actual.ParameterType == typeof(int) || actual.ParameterType == typeof(short) || actual.ParameterType == typeof(byte)))
					{
						result = true;
						return result;
					}
					if (formal.ParameterType == typeof(float) && (actual.ParameterType == typeof(long) || actual.ParameterType == typeof(int) || actual.ParameterType == typeof(short) || actual.ParameterType == typeof(byte)))
					{
						result = true;
						return result;
					}
					if (formal.ParameterType == typeof(double) && (actual.ParameterType == typeof(float) || actual.ParameterType == typeof(long) || actual.ParameterType == typeof(int) || actual.ParameterType == typeof(short) || actual.ParameterType == typeof(byte)))
					{
						result = true;
						return result;
					}
				}
				result = false;
			}
			return result;
		}

		private static string CreateDescriptiveAmbiguousErrorMessage(IList list, Type[] classes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("There are two or more methods that can be bound given the parameters types (");
			for (int i = 0; i < classes.Length; i++)
			{
				Type type = classes[i];
				if (type == null)
				{
					stringBuilder.Append("null");
				}
				else
				{
					stringBuilder.Append(type.Name);
				}
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(") Methods: ");
			foreach (MethodInfo methodInfo in list)
			{
				stringBuilder.AppendFormat(" {0}.{1}({2}) ", methodInfo.DeclaringType.Name, methodInfo.Name, MethodMap.CreateParametersDescription(methodInfo.GetParameters()));
			}
			return stringBuilder.ToString();
		}

		private static string CreateParametersDescription(ParameterInfo[] parameters)
		{
			string text = string.Empty;
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				if (text != string.Empty)
				{
					text += ", ";
				}
				text += parameterInfo.ParameterType.Name;
			}
			return text;
		}
	}
}
