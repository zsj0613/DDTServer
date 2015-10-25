using System;
using System.IO;
using System.Text;
using zlib;
namespace Game.Base
{
	public class Marshal
	{
		public static string ConvertToString(byte[] cstyle)
		{
			string result;
			if (cstyle == null)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < cstyle.Length; i++)
				{
					if (cstyle[i] == 0)
					{
						result = Encoding.Default.GetString(cstyle, 0, i);
						return result;
					}
				}
				result = Encoding.Default.GetString(cstyle);
			}
			return result;
		}

        public static long ConvertToInt64(int v1, uint v2)
        {
            int num = 1;
            if (v1 < 0)
            {
                num = -1;
            }
            return (long)(num * (Math.Abs(v1 * Math.Pow(2.0, 32.0)) + v2));
        }

        public static int ConvertToInt32(byte[] val)
		{
			return Marshal.ConvertToInt32(val, 0);
		}
		public static int ConvertToInt32(byte[] val, int startIndex)
		{
			return Marshal.ConvertToInt32(val[startIndex], val[startIndex + 1], val[startIndex + 2], val[startIndex + 3]);
		}
		public static int ConvertToInt32(byte v1, byte v2, byte v3, byte v4)
		{
			return (int)v1 << 24 | (int)v2 << 16 | (int)v3 << 8 | (int)v4;
		}
		public static uint ConvertToUInt32(byte[] val)
		{
			return Marshal.ConvertToUInt32(val, 0);
		}
		public static uint ConvertToUInt32(byte[] val, int startIndex)
		{
			return Marshal.ConvertToUInt32(val[startIndex], val[startIndex + 1], val[startIndex + 2], val[startIndex + 3]);
		}
		public static uint ConvertToUInt32(byte v1, byte v2, byte v3, byte v4)
		{
			return (uint)((int)v1 << 24 | (int)v2 << 16 | (int)v3 << 8 | (int)v4);
		}
		public static short ConvertToInt16(byte[] val)
		{
			return Marshal.ConvertToInt16(val, 0);
		}
		public static short ConvertToInt16(byte[] val, int startIndex)
		{
			return Marshal.ConvertToInt16(val[startIndex], val[startIndex + 1]);
		}
		public static short ConvertToInt16(byte v1, byte v2)
		{
			return (short)((int)v1 << 8 | (int)v2);
		}
		public static ushort ConvertToUInt16(byte[] val)
		{
			return Marshal.ConvertToUInt16(val, 0);
		}
		public static ushort ConvertToUInt16(byte[] val, int startIndex)
		{
			return Marshal.ConvertToUInt16(val[startIndex], val[startIndex + 1]);
		}
		public static ushort ConvertToUInt16(byte v1, byte v2)
		{
			return (ushort)((int)v2 | (int)v1 << 8);
		}
		public static string ToHexDump(string description, byte[] dump)
		{
			return Marshal.ToHexDump(description, dump, 0, dump.Length);
		}
		public static string ToHexDump(string description, byte[] dump, int start, int count)
		{
			StringBuilder hexDump = new StringBuilder();
			if (description != null)
			{
				hexDump.Append(description).Append("\n");
			}
			int end = start + count;
			for (int i = start; i < end; i += 16)
			{
				StringBuilder text = new StringBuilder();
				StringBuilder hex = new StringBuilder();
				hex.Append(i.ToString("X4"));
				hex.Append(": ");
				for (int j = 0; j < 16; j++)
				{
					if (j + i < end)
					{
						byte val = dump[j + i];
						hex.Append(dump[j + i].ToString("X2"));
						hex.Append(" ");
						if (val >= 32 && val <= 127)
						{
							text.Append((char)val);
						}
						else
						{
							text.Append(".");
						}
					}
					else
					{
						hex.Append("   ");
						text.Append(" ");
					}
				}
				hex.Append("  ");
				hex.Append(text.ToString());
				hex.Append('\n');
				hexDump.Append(hex.ToString());
			}
			return hexDump.ToString();
		}
		public static byte[] Compress(byte[] src)
		{
			return Marshal.Compress(src, 0, src.Length);
		}
		public static byte[] Compress(byte[] src, int offset, int length)
		{
			MemoryStream ms = new MemoryStream();
			Stream s = new ZOutputStream(ms, 9);
			s.Write(src, offset, length);
			s.Close();
			return ms.ToArray();
		}
		public static byte[] Uncompress(byte[] src)
		{
			MemoryStream md = new MemoryStream();
			Stream d = new ZOutputStream(md);
			d.Write(src, 0, src.Length);
			d.Close();
			return md.ToArray();
		}

        internal static void FreeHGlobal(IntPtr centerintptr)
        {
            throw new NotImplementedException();
        }
    }
}
