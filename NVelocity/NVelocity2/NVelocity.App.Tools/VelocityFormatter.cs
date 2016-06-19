using NVelocity.Context;
using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace NVelocity.App.Tools
{
	public class VelocityFormatter
	{
		public class VelocityAlternator
		{
			protected internal string[] alternates = null;

			protected internal int current = 0;

			public VelocityAlternator(params string[] alternates)
			{
				this.alternates = alternates;
			}

			public string Alternate()
			{
				this.current++;
				this.current %= this.alternates.Length;
				return "";
			}

			public override string ToString()
			{
				return this.alternates[this.current];
			}
		}

		public class VelocityAutoAlternator : VelocityFormatter.VelocityAlternator
		{
			public VelocityAutoAlternator(params string[] alternates) : base(alternates)
			{
			}

			public override string ToString()
			{
				string result = this.alternates[this.current];
				base.Alternate();
				return result;
			}
		}

		internal IContext context = null;

		internal SupportClass.TextNumberFormat nf = SupportClass.TextNumberFormat.getTextNumberInstance();

		public VelocityFormatter(IContext context)
		{
			this.context = context;
		}

		public string FormatShortDate(DateTime date)
		{
			return SupportClass.FormatDateTime(SupportClass.GetDateTimeFormatInstance(3, -1, CultureInfo.CurrentCulture), date);
		}

		public string FormatLongDate(DateTime date)
		{
			return SupportClass.FormatDateTime(SupportClass.GetDateTimeFormatInstance(1, -1, CultureInfo.CurrentCulture), date);
		}

		public string FormatShortDateTime(DateTime date)
		{
			return SupportClass.FormatDateTime(SupportClass.GetDateTimeFormatInstance(3, 3, CultureInfo.CurrentCulture), date);
		}

		public string FormatLongDateTime(DateTime date)
		{
			return SupportClass.FormatDateTime(SupportClass.GetDateTimeFormatInstance(1, 1, CultureInfo.CurrentCulture), date);
		}

		public string FormatArray(object array)
		{
			return this.FormatArray(array, ", ", " and ");
		}

		public string FormatArray(object array, string delim)
		{
			return this.FormatArray(array, delim, delim);
		}

		public string FormatArray(object array, string delim, string finaldelim)
		{
			Array array2 = (Array)array;
			StringBuilder stringBuilder = new StringBuilder();
			int num = ((double[])array).Length;
			for (int i = 0; i < num; i++)
			{
				stringBuilder.Append(array2.GetValue(i).ToString());
				if (i < num - 2)
				{
					stringBuilder.Append(delim);
				}
				else if (i < num - 1)
				{
					stringBuilder.Append(finaldelim);
				}
			}
			return stringBuilder.ToString();
		}

		public string FormatVector(IList list)
		{
			return this.FormatVector(list, ", ", " and ");
		}

		public string FormatVector(IList list, string delim)
		{
			return this.FormatVector(list, delim, delim);
		}

		public string FormatVector(IList list, string delim, string finaldelim)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				stringBuilder.Append(list[i].ToString());
				if (i < count - 2)
				{
					stringBuilder.Append(delim);
				}
				else if (i < count - 1)
				{
					stringBuilder.Append(finaldelim);
				}
			}
			return stringBuilder.ToString();
		}

		public string LimitLen(int maxlen, string value)
		{
			return this.LimitLen(maxlen, value, "...");
		}

		public string LimitLen(int maxlen, string value, string suffix)
		{
			string result = value;
			if (value.Length > maxlen)
			{
				result = value.Substring(0, maxlen - suffix.Length) + suffix;
			}
			return result;
		}

		public string MakeAlternator(string name, string alt1, string alt2)
		{
			this.context.Put(name, new VelocityFormatter.VelocityAlternator(new string[]
			{
				alt1,
				alt2
			}));
			return "";
		}

		public string MakeAlternator(string name, string alt1, string alt2, string alt3)
		{
			this.context.Put(name, new VelocityFormatter.VelocityAlternator(new string[]
			{
				alt1,
				alt2,
				alt3
			}));
			return "";
		}

		public string MakeAlternator(string name, string alt1, string alt2, string alt3, string alt4)
		{
			this.context.Put(name, new VelocityFormatter.VelocityAlternator(new string[]
			{
				alt1,
				alt2,
				alt3,
				alt4
			}));
			return "";
		}

		public string MakeAutoAlternator(string name, string alt1, string alt2)
		{
			this.context.Put(name, new VelocityFormatter.VelocityAutoAlternator(new string[]
			{
				alt1,
				alt2
			}));
			return "";
		}

		public object IsNull(object o, object dflt)
		{
			object result;
			if (o == null)
			{
				result = dflt;
			}
			else
			{
				result = o;
			}
			return result;
		}
	}
}
