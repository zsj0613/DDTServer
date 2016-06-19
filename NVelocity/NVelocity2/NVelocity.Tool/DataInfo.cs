using System;

namespace NVelocity.Tool
{
	public class DataInfo : IToolInfo
	{
		public static readonly string TYPE_STRING = "string";

		public static readonly string TYPE_NUMBER = "number";

		public static readonly string TYPE_BOOLEAN = "boolean";

		private string key;

		private object data;

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		public string Classname
		{
			get
			{
				return this.data.GetType().FullName;
			}
		}

		public DataInfo(string key, string type, string value_)
		{
			this.key = key;
			if (type.ToUpper().Equals(DataInfo.TYPE_BOOLEAN.ToUpper()))
			{
				this.data = bool.Parse(value_);
			}
			else if (type.ToUpper().Equals(DataInfo.TYPE_NUMBER.ToUpper()))
			{
				if (value_.IndexOf('.') >= 0)
				{
					this.data = double.Parse(value_);
				}
				else
				{
					this.data = int.Parse(value_);
				}
			}
			else
			{
				this.data = value_;
			}
		}

		public object getInstance(object initData)
		{
			return this.data;
		}
	}
}
