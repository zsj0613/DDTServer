using System;

namespace NVelocity.Runtime.Parser.Node
{
	public class MathUtil
	{
		public static object Add(Type maxType, object left, object right)
		{
			object result;
			if (maxType == typeof(double))
			{
				result = Convert.ToDouble(left) + Convert.ToDouble(right);
			}
			else if (maxType == typeof(float))
			{
				result = Convert.ToSingle(left) + Convert.ToSingle(right);
			}
			else if (maxType == typeof(decimal))
			{
				result = Convert.ToDecimal(left) + Convert.ToDecimal(right);
			}
			else if (maxType == typeof(long))
			{
				result = Convert.ToInt64(left) + Convert.ToInt64(right);
			}
			else if (maxType == typeof(int))
			{
				result = Convert.ToInt32(left) + Convert.ToInt32(right);
			}
			else if (maxType == typeof(short))
			{
				result = (int)(Convert.ToInt16(left) + Convert.ToInt16(right));
			}
			else if (maxType == typeof(sbyte))
			{
				result = (int)(Convert.ToSByte(left) + Convert.ToSByte(right));
			}
			else if (maxType == typeof(byte))
			{
				result = (int)(Convert.ToByte(left) + Convert.ToByte(right));
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static object Mult(Type maxType, object left, object right)
		{
			object result;
			if (maxType == typeof(double))
			{
				result = Convert.ToDouble(left) * Convert.ToDouble(right);
			}
			else if (maxType == typeof(float))
			{
				result = Convert.ToSingle(left) * Convert.ToSingle(right);
			}
			else if (maxType == typeof(decimal))
			{
				result = Convert.ToDecimal(left) * Convert.ToDecimal(right);
			}
			else if (maxType == typeof(long))
			{
				result = Convert.ToInt64(left) * Convert.ToInt64(right);
			}
			else if (maxType == typeof(int))
			{
				result = Convert.ToInt32(left) * Convert.ToInt32(right);
			}
			else if (maxType == typeof(short))
			{
				result = (int)(Convert.ToInt16(left) * Convert.ToInt16(right));
			}
			else if (maxType == typeof(sbyte))
			{
				result = (int)(Convert.ToSByte(left) * Convert.ToSByte(right));
			}
			else if (maxType == typeof(byte))
			{
				result = (int)(Convert.ToByte(left) * Convert.ToByte(right));
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static object Div(Type maxType, object left, object right)
		{
			object result;
			if (maxType == typeof(double))
			{
				result = Convert.ToDouble(left) / Convert.ToDouble(right);
			}
			else if (maxType == typeof(float))
			{
				result = Convert.ToSingle(left) / Convert.ToSingle(right);
			}
			else if (maxType == typeof(decimal))
			{
				result = Convert.ToDecimal(left) / Convert.ToDecimal(right);
			}
			else if (maxType == typeof(long))
			{
				result = Convert.ToInt64(left) / Convert.ToInt64(right);
			}
			else if (maxType == typeof(int))
			{
				result = Convert.ToInt32(left) / Convert.ToInt32(right);
			}
			else if (maxType == typeof(short))
			{
				result = (int)(Convert.ToInt16(left) / Convert.ToInt16(right));
			}
			else if (maxType == typeof(sbyte))
			{
				result = (int)(Convert.ToSByte(left) / Convert.ToSByte(right));
			}
			else if (maxType == typeof(byte))
			{
				result = (int)(Convert.ToByte(left) / Convert.ToByte(right));
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static object Mod(Type maxType, object left, object right)
		{
			object result;
			if (maxType == typeof(double))
			{
				result = Convert.ToDouble(left) % Convert.ToDouble(right);
			}
			else if (maxType == typeof(float))
			{
				result = Convert.ToSingle(left) % Convert.ToSingle(right);
			}
			else if (maxType == typeof(decimal))
			{
				result = Convert.ToDecimal(left) % Convert.ToDecimal(right);
			}
			else if (maxType == typeof(long))
			{
				result = Convert.ToInt64(left) % Convert.ToInt64(right);
			}
			else if (maxType == typeof(int))
			{
				result = Convert.ToInt32(left) % Convert.ToInt32(right);
			}
			else if (maxType == typeof(short))
			{
				result = (int)(Convert.ToInt16(left) % Convert.ToInt16(right));
			}
			else if (maxType == typeof(sbyte))
			{
				result = (int)(Convert.ToSByte(left) % Convert.ToSByte(right));
			}
			else if (maxType == typeof(byte))
			{
				result = (int)(Convert.ToByte(left) % Convert.ToByte(right));
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static object Sub(Type maxType, object left, object right)
		{
			object result;
			if (maxType == typeof(double))
			{
				result = Convert.ToDouble(left) - Convert.ToDouble(right);
			}
			else if (maxType == typeof(float))
			{
				result = Convert.ToSingle(left) - Convert.ToSingle(right);
			}
			else if (maxType == typeof(decimal))
			{
				result = Convert.ToDecimal(left) - Convert.ToDecimal(right);
			}
			else if (maxType == typeof(long))
			{
				result = Convert.ToInt64(left) - Convert.ToInt64(right);
			}
			else if (maxType == typeof(int))
			{
				result = Convert.ToInt32(left) - Convert.ToInt32(right);
			}
			else if (maxType == typeof(short))
			{
				result = (int)(Convert.ToInt16(left) - Convert.ToInt16(right));
			}
			else if (maxType == typeof(sbyte))
			{
				result = (int)(Convert.ToSByte(left) - Convert.ToSByte(right));
			}
			else if (maxType == typeof(byte))
			{
				result = (int)(Convert.ToByte(left) - Convert.ToByte(right));
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static Type ToMaxType(Type leftType, Type rightType)
		{
			Type result;
			if (leftType == rightType)
			{
				result = leftType;
			}
			else if (leftType == typeof(double) || rightType == typeof(double))
			{
				result = typeof(double);
			}
			else if (leftType == typeof(float) || rightType == typeof(float))
			{
				result = typeof(float);
			}
			else if (leftType == typeof(decimal) || rightType == typeof(decimal))
			{
				result = typeof(decimal);
			}
			else if (leftType == typeof(long) || rightType == typeof(long))
			{
				result = typeof(long);
			}
			else if (leftType == typeof(int) || rightType == typeof(int))
			{
				result = typeof(int);
			}
			else if (leftType == typeof(short) || rightType == typeof(short))
			{
				result = typeof(short);
			}
			else if (leftType == typeof(sbyte) || rightType == typeof(sbyte))
			{
				result = typeof(sbyte);
			}
			else if (leftType == typeof(byte) || rightType == typeof(byte))
			{
				result = typeof(byte);
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
