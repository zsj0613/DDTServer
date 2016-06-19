using NVelocity.Runtime;
using NVelocity.Runtime.Parser.Node;
using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace NVelocity.Util.Introspection
{
	public class UberspectImpl : IUberspect, UberspectLoggable
	{
		public class VelMethodImpl : IVelMethod
		{
			internal MethodInfo method = null;

			public bool Cacheable
			{
				get
				{
					return true;
				}
			}

			public string MethodName
			{
				get
				{
					return this.method.Name;
				}
			}

			public Type ReturnType
			{
				get
				{
					return this.method.ReturnType;
				}
			}

			public VelMethodImpl(MethodInfo m)
			{
				this.method = m;
			}

			public object Invoke(object o, object[] parameters)
			{
				return this.method.Invoke(o, parameters);
			}
		}

		public class VelGetterImpl : IVelPropertyGet
		{
			internal AbstractExecutor ae = null;

			public bool Cacheable
			{
				get
				{
					return true;
				}
			}

			public string MethodName
			{
				get
				{
					string result;
					if (this.ae.Property.Name != null)
					{
						result = this.ae.Property.Name;
					}
					else if (this.ae.Method != null)
					{
						result = this.ae.Method.Name;
					}
					else
					{
						result = "undefined";
					}
					return result;
				}
			}

			public VelGetterImpl(AbstractExecutor exec)
			{
				this.ae = exec;
			}

			public object Invoke(object o)
			{
				return this.ae.Execute(o);
			}
		}

		public class VelSetterImpl : IVelPropertySet
		{
			internal IVelMethod vm = null;

			internal string putKey = null;

			public bool Cacheable
			{
				get
				{
					return true;
				}
			}

			public string MethodName
			{
				get
				{
					return this.vm.MethodName;
				}
			}

			public VelSetterImpl(IVelMethod velmethod)
			{
				this.vm = velmethod;
			}

			public VelSetterImpl(IVelMethod velmethod, string key)
			{
				this.vm = velmethod;
				this.putKey = key;
			}

			public object Invoke(object o, object value)
			{
				ArrayList arrayList = new ArrayList();
				if (this.putKey != null)
				{
					arrayList.Add(this.putKey);
					arrayList.Add(value);
				}
				else
				{
					arrayList.Add(value);
				}
				return this.vm.Invoke(o, arrayList.ToArray());
			}
		}

		private IRuntimeLogger rlog;

		private static Introspector introspector;

		public IRuntimeLogger RuntimeLogger
		{
			set
			{
				this.rlog = value;
				UberspectImpl.introspector = new Introspector(this.rlog);
			}
		}

		public void Init()
		{
		}

		public IVelMethod GetMethod(object obj, string methodName, object[] args, Info i)
		{
			IVelMethod result;
			if (obj == null)
			{
				result = null;
			}
			else
			{
				MethodInfo method = UberspectImpl.introspector.GetMethod(obj.GetType(), methodName, args);
				result = ((method != null) ? new UberspectImpl.VelMethodImpl(method) : null);
			}
			return result;
		}

		public IVelPropertyGet GetPropertyGet(object obj, string identifier, Info i)
		{
			Type type = obj.GetType();
			AbstractExecutor abstractExecutor = new PropertyExecutor(this.rlog, UberspectImpl.introspector, type, identifier);
			if (!abstractExecutor.IsAlive)
			{
				abstractExecutor = new GetExecutor(this.rlog, UberspectImpl.introspector, type, identifier);
			}
			if (!abstractExecutor.IsAlive)
			{
				abstractExecutor = new BooleanPropertyExecutor(this.rlog, UberspectImpl.introspector, type, identifier);
			}
			return (abstractExecutor != null) ? new UberspectImpl.VelGetterImpl(abstractExecutor) : null;
		}

		public IVelPropertySet GetPropertySet(object obj, string identifier, object arg, Info i)
		{
			Type type = obj.GetType();
			IVelMethod velMethod = null;
			IVelPropertySet result;
			try
			{
				object[] args = new object[]
				{
					arg
				};
				try
				{
					velMethod = this.GetMethod(obj, "set" + identifier, args, i);
					if (velMethod == null)
					{
						throw new MethodAccessException();
					}
				}
				catch (MethodAccessException)
				{
					StringBuilder stringBuilder = new StringBuilder("set");
					stringBuilder.Append(identifier);
					if (char.IsLower(stringBuilder[3]))
					{
						stringBuilder[3] = char.ToUpper(stringBuilder[3]);
					}
					else
					{
						stringBuilder[3] = char.ToLower(stringBuilder[3]);
					}
					velMethod = this.GetMethod(obj, stringBuilder.ToString(), args, i);
					if (velMethod == null)
					{
						throw;
					}
				}
			}
			catch (MethodAccessException)
			{
				if (typeof(IDictionary).IsAssignableFrom(type))
				{
					object[] args = new object[]
					{
						new object(),
						new object()
					};
					velMethod = this.GetMethod(obj, "Add", args, i);
					if (velMethod != null)
					{
						result = new UberspectImpl.VelSetterImpl(velMethod, identifier);
						return result;
					}
				}
			}
			result = ((velMethod != null) ? new UberspectImpl.VelSetterImpl(velMethod) : null);
			return result;
		}
	}
}
