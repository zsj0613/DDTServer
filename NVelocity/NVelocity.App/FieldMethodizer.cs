using System;
using System.Collections;
using System.Reflection;

namespace NVelocity.App
{
	public class FieldMethodizer
	{
		private Hashtable fieldHash = new Hashtable();

		private Hashtable classHash = new Hashtable();

		public FieldMethodizer()
		{
		}

		public FieldMethodizer(string s)
		{
			try
			{
				this.AddObject(s);
			}
			catch (System.Exception value)
			{
				Console.Out.WriteLine(value);
			}
		}

		public FieldMethodizer(object o)
		{
			try
			{
				this.AddObject(o);
			}
			catch (System.Exception value)
			{
				Console.Out.WriteLine(value);
			}
		}

		public void AddObject(string s)
		{
			Type type = Type.GetType(s);
			this.Inspect(type);
		}

		public void AddObject(object o)
		{
			this.Inspect(o.GetType());
		}

		public object Get(string fieldName)
		{
			object result;
			try
			{
				FieldInfo fieldInfo = (FieldInfo)this.fieldHash[fieldName];
				if (fieldInfo != null)
				{
					result = fieldInfo.GetValue((Type)this.classHash[fieldName]);
					return result;
				}
			}
			catch (System.Exception var_1_39)
			{
			}
			result = null;
			return result;
		}

		private void Inspect(Type clas)
		{
			FieldInfo[] fields = clas.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i].IsPublic && fields[i].IsStatic)
				{
					this.fieldHash[fields[i].Name] = fields[i];
					this.classHash[fields[i].Name] = clas;
				}
			}
		}
	}
}
