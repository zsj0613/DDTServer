using System;
using System.Collections;

namespace NVelocity.Runtime.Parser.Node
{
	public class ObjectComparer : IComparer
	{
		private interface IObjectComparer
		{
			void Register(IDictionary map);

			int Compare(object x, object y);
		}

		private class DoubleComparer : ObjectComparer.IObjectComparer
		{
			public void Register(IDictionary map)
			{
				map[typeof(double) + ":" + typeof(long)] = this;
				map[typeof(long) + ":" + typeof(double)] = this;
				map[typeof(double) + ":" + typeof(ulong)] = this;
				map[typeof(ulong) + ":" + typeof(double)] = this;
				map[typeof(float) + ":" + typeof(double)] = this;
				map[typeof(double) + ":" + typeof(float)] = this;
			}

			public int Compare(object x, object y)
			{
				int result;
				if (x is double)
				{
					result = this.Compare((double)x, y);
				}
				else
				{
					result = -this.Compare((double)y, x);
				}
				return result;
			}

			public int Compare(double d, object y)
			{
				int result;
				if (y is long)
				{
					long num = (long)y;
					result = ((d == (double)num) ? 0 : ((d < (double)num) ? -1 : 1));
				}
				else if (y is ulong)
				{
					ulong num2 = (ulong)y;
					result = ((d == num2) ? 0 : ((d < num2) ? -1 : 1));
				}
				else
				{
					if (!(y is float))
					{
						throw new ArgumentException("Unable to compare double and " + y.GetType());
					}
					float num3 = (float)y;
					result = ((d == (double)num3) ? 0 : ((d < (double)num3) ? -1 : 1));
				}
				return result;
			}
		}

		private class FloatComparer : ObjectComparer.IObjectComparer
		{
			public void Register(IDictionary map)
			{
				map[typeof(float) + ":" + typeof(ulong)] = this;
				map[typeof(ulong) + ":" + typeof(float)] = this;
				map[typeof(float) + ":" + typeof(long)] = this;
				map[typeof(long) + ":" + typeof(float)] = this;
			}

			public int Compare(object x, object y)
			{
				int result;
				if (x is float)
				{
					result = this.Compare((float)x, y);
				}
				else
				{
					result = -this.Compare((float)y, x);
				}
				return result;
			}

			public int Compare(float f, object y)
			{
				int result;
				if (y is long)
				{
					long num = (long)y;
					result = ((f == (float)num) ? 0 : ((f < (float)num) ? -1 : 1));
				}
				else
				{
					if (!(y is ulong))
					{
						throw new ArgumentException("Unable to compare float and " + y.GetType());
					}
					ulong num2 = (ulong)y;
					result = ((f == num2) ? 0 : ((f < num2) ? -1 : 1));
				}
				return result;
			}
		}

		private class ULongComparer : ObjectComparer.IObjectComparer
		{
			public void Register(IDictionary map)
			{
				map[typeof(long) + ":" + typeof(ulong)] = this;
				map[typeof(ulong) + ":" + typeof(long)] = this;
			}

			public int Compare(object x, object y)
			{
				int result;
				if (x is ulong)
				{
					result = this.Compare((ulong)x, y);
				}
				else
				{
					result = -this.Compare((ulong)y, x);
				}
				return result;
			}

			public int Compare(ulong ul, object y)
			{
				if (y is long)
				{
					long num = (long)y;
					int result;
					if (num < 0L)
					{
						result = -1;
					}
					else
					{
						ulong num2 = (ulong)num;
						result = ((ul == num2) ? 0 : ((ul < num2) ? -1 : 1));
					}
					return result;
				}
				throw new ArgumentException("Unable to compare long and " + y.GetType());
			}
		}

		public const int Smaller = -1;

		public const int Equal = 0;

		public const int Greater = 1;

		private static readonly IDictionary comparers;

		private static readonly ObjectComparer instance;

		static ObjectComparer()
		{
			ObjectComparer.comparers = new Hashtable();
			ObjectComparer.instance = new ObjectComparer();
			new ObjectComparer.DoubleComparer().Register(ObjectComparer.comparers);
			new ObjectComparer.FloatComparer().Register(ObjectComparer.comparers);
			new ObjectComparer.ULongComparer().Register(ObjectComparer.comparers);
		}

		public static int CompareObjects(object x, object y)
		{
			return ObjectComparer.instance.Compare(x, y);
		}

		public int Compare(object x, object y)
		{
			int result;
			if (x != null)
			{
				Type type = x.GetType();
				if (y != null)
				{
					Type type2 = y.GetType();
					if (x is string || y is string)
					{
						result = string.Compare(x.ToString(), y.ToString());
					}
					else if (type.IsPrimitive && type2.IsPrimitive)
					{
						result = this.ComparePrimitive(x, y);
					}
					else if (type == typeof(decimal) || type2 == typeof(decimal))
					{
						result = decimal.Compare(Convert.ToDecimal(x), Convert.ToDecimal(y));
					}
					else if (x is IComparable)
					{
						result = (x as IComparable).CompareTo(y);
					}
					else
					{
						if (!(y is IComparable))
						{
							throw new ArgumentException(string.Concat(new object[]
							{
								"Unable to compare ",
								x,
								" and ",
								y
							}));
						}
						result = -(y as IComparable).CompareTo(x);
					}
				}
				else
				{
					result = 1;
				}
			}
			else
			{
				result = -1;
			}
			return result;
		}

		public int ComparePrimitive(object x, object y)
		{
			x = this.ReType(x);
			y = this.ReType(y);
			int result;
			if (x.GetType() == y.GetType())
			{
				result = (x as IComparable).CompareTo(y);
			}
			else
			{
				ObjectComparer.IObjectComparer objectComparer = ObjectComparer.comparers[x.GetType() + ":" + y.GetType()] as ObjectComparer.IObjectComparer;
				if (objectComparer == null)
				{
					throw new ArgumentException(string.Concat(new object[]
					{
						"Unable to compare ",
						x.GetType(),
						" and ",
						y.GetType()
					}));
				}
				result = objectComparer.Compare(x, y);
			}
			return result;
		}

		public object ReType(object value)
		{
			if (value is char || value is byte || value is sbyte || value is short || value is ushort || value is int || value is uint || value is long)
			{
				value = Convert.ToInt64(value);
			}
			return value;
		}
	}
}
