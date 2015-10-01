using NVelocity.App.Events;
using NVelocity.Context;
using NVelocity.Exception;
using NVelocity.Util.Introspection;
using System;
using System.Reflection;

namespace NVelocity.Runtime.Parser.Node
{
	public class ASTMethod : SimpleNode
	{
		private string methodName = "";

		private int paramCount = 0;

		private object[] parameters;

		private int paramArrayIndex = -1;

		public ASTMethod(int id) : base(id)
		{
		}

		public ASTMethod(Parser p, int id) : base(p, id)
		{
		}

		public override object Accept(IParserVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}

		public override object Init(IInternalContextAdapter context, object data)
		{
			base.Init(context, data);
			this.methodName = base.FirstToken.Image;
			this.paramCount = base.ChildrenCount - 1;
			this.parameters = new object[this.paramCount];
			return data;
		}

		private object doIntrospection(IInternalContextAdapter context, Type data)
		{
			for (int i = 0; i < this.paramCount; i++)
			{
				this.parameters[i] = base.GetChild(i + 1).Value(context);
			}
			string name = this.methodName;
			MethodInfo method = this.rsvc.Introspector.GetMethod(data, name, this.parameters);
			PropertyInfo propertyInfo = null;
			if (method == null)
			{
				name = this.methodName.Substring(0, 1).ToUpper() + this.methodName.Substring(1);
				method = this.rsvc.Introspector.GetMethod(data, name, this.parameters);
				if (method == null)
				{
					name = this.methodName.Substring(0, 1).ToLower() + this.methodName.Substring(1);
					method = this.rsvc.Introspector.GetMethod(data, name, this.parameters);
					if (method == null && this.paramCount == 0)
					{
						name = this.methodName;
						propertyInfo = this.rsvc.Introspector.GetProperty(data, name);
						if (propertyInfo == null)
						{
							name = this.methodName.Substring(0, 1).ToUpper() + this.methodName.Substring(1);
							propertyInfo = this.rsvc.Introspector.GetProperty(data, name);
							if (propertyInfo == null)
							{
								name = this.methodName.Substring(0, 1).ToLower() + this.methodName.Substring(1);
								propertyInfo = this.rsvc.Introspector.GetProperty(data, name);
							}
						}
					}
				}
			}
			object result;
			if (method != null)
			{
				result = method;
			}
			else
			{
				result = propertyInfo;
			}
			return result;
		}

		public override object Execute(object o, IInternalContextAdapter context)
		{
			MethodInfo methodInfo = null;
			PropertyInfo propertyInfo = null;
			bool flag = false;
			object[] array = this.parameters;
			object result;
			try
			{
				IntrospectionCacheData introspectionCacheData = context.ICacheGet(this);
				Type type = o.GetType();
				if (introspectionCacheData != null && introspectionCacheData.ContextData == type)
				{
					for (int i = 0; i < this.paramCount; i++)
					{
						this.parameters[i] = base.GetChild(i + 1).Value(context);
					}
					flag = true;
					if (introspectionCacheData.Thingy is MethodInfo)
					{
						methodInfo = (MethodInfo)introspectionCacheData.Thingy;
						array = this.BuildMethodArgs(methodInfo, this.paramArrayIndex);
					}
					if (introspectionCacheData.Thingy is PropertyInfo)
					{
						propertyInfo = (PropertyInfo)introspectionCacheData.Thingy;
					}
				}
				else
				{
					object obj = this.doIntrospection(context, type);
					if (obj is MethodInfo)
					{
						methodInfo = (MethodInfo)obj;
					}
					if (obj is PropertyInfo)
					{
						propertyInfo = (PropertyInfo)obj;
					}
					if (obj != null)
					{
						context.ICachePut(this, new IntrospectionCacheData
						{
							ContextData = type,
							Thingy = obj
						});
					}
				}
				if (methodInfo == null && propertyInfo == null)
				{
					result = null;
					return result;
				}
			}
			catch (System.Exception ex)
			{
				this.rsvc.Error("ASTMethod.execute() : exception from introspection : " + ex);
				throw new RuntimeException(string.Format("Error during object instrospection. Check inner exception for details. Node literal {0} Line {1} Column {2}", base.Literal, base.Line, base.Column), ex);
			}
			try
			{
				object obj;
				if (methodInfo != null)
				{
					if (!flag)
					{
						array = this.BuildMethodArgs(methodInfo);
					}
					obj = methodInfo.Invoke(o, array);
					if (obj == null && methodInfo.ReturnType == typeof(void))
					{
						obj = string.Empty;
					}
				}
				else
				{
					obj = propertyInfo.GetValue(o, null);
				}
				result = obj;
			}
			catch (TargetInvocationException ex2)
			{
				EventCartridge eventCartridge = context.EventCartridge;
				if (eventCartridge != null)
				{
					try
					{
						result = eventCartridge.HandleMethodException(o.GetType(), this.methodName, ex2.GetBaseException());
						return result;
					}
					catch (System.Exception ex3)
					{
						throw new MethodInvocationException(string.Concat(new object[]
						{
							"Invocation of method '",
							this.methodName,
							"' in  ",
							o.GetType(),
							" threw exception ",
							ex3.GetType(),
							" : ",
							ex3.Message
						}), ex3, this.methodName);
					}
				}
				throw new MethodInvocationException(string.Concat(new object[]
				{
					"Invocation of method '",
					this.methodName,
					"' in  ",
					o.GetType(),
					" threw exception ",
					ex2.GetBaseException().GetType(),
					" : ",
					ex2.GetBaseException().Message
				}), ex2.GetBaseException(), this.methodName);
			}
			catch (System.Exception ex3)
			{
				this.rsvc.Error(string.Concat(new object[]
				{
					"ASTMethod.execute() : exception invoking method '",
					this.methodName,
					"' in ",
					o.GetType(),
					" : ",
					ex3
				}));
				throw ex3;
			}
			return result;
		}

		private object[] BuildMethodArgs(MethodInfo method, int paramArrayIndex)
		{
			object[] result = this.parameters;
			ParameterInfo[] array = method.GetParameters();
			if (paramArrayIndex != -1)
			{
				Type parameterType = array[paramArrayIndex].ParameterType;
				object[] array2 = new object[array.Length];
				Array.Copy(this.parameters, array2, array.Length - 1);
				if (this.parameters.Length < paramArrayIndex + 1)
				{
					array2[paramArrayIndex] = Array.CreateInstance(parameterType.GetElementType(), 0);
				}
				else
				{
					Array array3 = Array.CreateInstance(parameterType.GetElementType(), this.parameters.Length + 1 - array2.Length);
					Array.Copy(this.parameters, array.Length - 1, array3, 0, array3.Length);
					array2[paramArrayIndex] = array3;
				}
				result = array2;
			}
			return result;
		}

		private object[] BuildMethodArgs(MethodInfo method)
		{
			ParameterInfo[] array = method.GetParameters();
			int num = -1;
			for (int i = 0; i < array.Length; i++)
			{
				ParameterInfo parameterInfo = array[i];
				if (parameterInfo.IsDefined(typeof(ParamArrayAttribute), false))
				{
					num = i;
					break;
				}
			}
			this.paramArrayIndex = num;
			return this.BuildMethodArgs(method, num);
		}
	}
}
