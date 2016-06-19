using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Commons.Collections
{
	public class ExtendedProperties : Hashtable
	{
		private static readonly byte DEFAULT_BYTE = 0;

		private static readonly bool DEFAULT_BOOLEAN = false;

		private static readonly short DEFAULT_INT16 = 0;

		private static readonly int DEFAULT_INT32 = 0;

		private static readonly float DEFAULT_SINGLE = 0f;

		private static readonly long DEFAULT_INT64 = 0L;

		private static readonly double DEFAULT_DOUBLE = 0.0;

		private ExtendedProperties defaults;

		protected internal string file;

		protected internal string basePath;

		protected internal string fileSeparator = "" + Path.AltDirectorySeparatorChar;

		protected internal bool isInitialized = false;

		protected internal static string include = "include";

		protected internal ArrayList keysAsListed = new ArrayList();

		public string Include
		{
			get
			{
				return ExtendedProperties.include;
			}
			set
			{
				ExtendedProperties.include = value;
			}
		}

		public new IEnumerable Keys
		{
			get
			{
				return this.keysAsListed;
			}
		}

		public ExtendedProperties()
		{
		}

		public ExtendedProperties(string file) : this(file, null)
		{
		}

		public ExtendedProperties(string file, string defaultFile)
		{
			this.file = file;
			this.basePath = new FileInfo(file).FullName;
			this.basePath = this.basePath.Substring(0, this.basePath.LastIndexOf(this.fileSeparator) + 1);
			this.Load(new FileStream(file, FileMode.Open, FileAccess.Read));
			if (defaultFile != null)
			{
				this.defaults = new ExtendedProperties(defaultFile);
			}
		}

		private void Init(ExtendedProperties exp)
		{
			this.isInitialized = true;
		}

		public bool IsInitialized()
		{
			return this.isInitialized;
		}

		public void Load(Stream input)
		{
			this.Load(input, null);
		}

		public void Load(Stream input, string enc)
		{
			lock (this)
			{
				PropertiesReader propertiesReader = null;
				if (enc != null)
				{
					try
					{
						propertiesReader = new PropertiesReader(new StreamReader(input, Encoding.GetEncoding(enc)));
					}
					catch
					{
					}
				}
				if (propertiesReader == null)
				{
					propertiesReader = new PropertiesReader(new StreamReader(input));
				}
				try
				{
					while (true)
					{
						string text = propertiesReader.ReadProperty();
						if (text == null)
						{
							break;
						}
						int num = text.IndexOf('=');
						if (num > 0)
						{
							string text2 = text.Substring(0, num).Trim();
							string text3 = text.Substring(num + 1).Trim();
							if (!string.Empty.Equals(text3))
							{
								if (this.Include != null && text2.ToUpper().Equals(this.Include.ToUpper()))
								{
									FileInfo fileInfo;
									if (text3.StartsWith(this.fileSeparator))
									{
										fileInfo = new FileInfo(text3);
									}
									else
									{
										if (text3.StartsWith("." + this.fileSeparator))
										{
											text3 = text3.Substring(2);
										}
										fileInfo = new FileInfo(this.basePath + text3);
									}
									bool flag = File.Exists(fileInfo.FullName) || Directory.Exists(fileInfo.FullName);
									if (fileInfo != null && flag)
									{
										this.Load(new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read));
									}
								}
								else
								{
									this.AddProperty(text2, text3);
								}
							}
						}
					}
				}

#pragma warning disable CS0168 // 声明了变量，但从未使用过
                catch (NullReferenceException var_8_1CD)
#pragma warning restore CS0168 // 声明了变量，但从未使用过
                {
					return;
				}
				propertiesReader.Close();
			}
		}

		public object GetProperty(string key)
		{
			object obj = this[key];
			if (obj == null)
			{
				if (this.defaults != null)
				{
					obj = this.defaults[key];
				}
			}
			return obj;
		}

		public void AddProperty(string key, object token)
		{
			object obj = this[key];
			if (obj is string)
			{
				CollectionsUtil.PutElement(this, key, new ArrayList(2)
				{
					obj,
					token
				});
			}
			else if (obj is ArrayList)
			{
				((ArrayList)obj).Add(token);
			}
			else if (token is string && ((string)token).IndexOf(",") > 0)
			{
				PropertiesTokenizer propertiesTokenizer = new PropertiesTokenizer((string)token);
				while (propertiesTokenizer.HasMoreTokens())
				{
					string token2 = propertiesTokenizer.NextToken();
					this.AddStringProperty(key, token2);
				}
			}
			else
			{
				this.AddPropertyDirect(key, token);
			}
		}

		private void AddPropertyDirect(string key, object obj)
		{
			if (!this.ContainsKey(key))
			{
				this.keysAsListed.Add(key);
			}
			CollectionsUtil.PutElement(this, key, obj);
		}

		private void AddStringProperty(string key, string token)
		{
			object obj = this[key];
			if (obj is string)
			{
				CollectionsUtil.PutElement(this, key, new ArrayList(2)
				{
					obj,
					token
				});
			}
			else if (obj is ArrayList)
			{
				((ArrayList)obj).Add(token);
			}
			else
			{
				this.AddPropertyDirect(key, token);
			}
		}

		public void SetProperty(string key, object value_)
		{
			this.ClearProperty(key);
			this.AddProperty(key, value_);
		}

		public void Save(TextWriter output, string Header)
		{
			lock (this)
			{
				if (output != null)
				{
					if (Header != null)
					{
						output.WriteLine(Header);
					}
					foreach (string key in this.Keys)
					{
						object obj = this[key];
						if (obj != null)
						{
							if (obj is string)
							{
								this.WriteKeyOutput(output, key, (string)obj);
							}
							else if (obj is IEnumerable)
							{
								foreach (string value in ((IEnumerable)obj))
								{
									this.WriteKeyOutput(output, key, value);
								}
							}
							output.WriteLine();
							output.Flush();
						}
					}
				}
			}
		}

		private void WriteKeyOutput(TextWriter theWrtr, string key, string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(key).Append("=").Append(value);
			theWrtr.WriteLine(stringBuilder.ToString());
		}

		public void Combine(ExtendedProperties c)
		{
			foreach (string key in c.Keys)
			{
				object obj = c[key];
				if (obj is string)
				{
					obj = ((string)obj).Replace(",", "\\,");
				}
				this.SetProperty(key, obj);
			}
		}

		public void ClearProperty(string key)
		{
			if (this.ContainsKey(key))
			{
				for (int i = 0; i < this.keysAsListed.Count; i++)
				{
					if (((string)this.keysAsListed[i]).Equals(key))
					{
						this.keysAsListed.RemoveAt(i);
						break;
					}
				}
				this.Remove(key);
			}
		}

		public IEnumerable GetKeys(string prefix)
		{
			ArrayList arrayList = new ArrayList();
			foreach (object current in this.Keys)
			{
				if (current is string && ((string)current).StartsWith(prefix))
				{
					arrayList.Add(current);
				}
			}
			return arrayList;
		}

		public ExtendedProperties Subset(string prefix)
		{
			ExtendedProperties extendedProperties = new ExtendedProperties();
			bool flag = false;
			foreach (object current in this.Keys)
			{
				if (current is string && ((string)current).StartsWith(prefix))
				{
					if (!flag)
					{
						flag = true;
					}
					string key;
					if (((string)current).Length == prefix.Length)
					{
						key = prefix;
					}
					else
					{
						key = ((string)current).Substring(prefix.Length + 1);
					}
					extendedProperties.AddPropertyDirect(key, this[current]);
				}
			}
			ExtendedProperties result;
			if (flag)
			{
				result = extendedProperties;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text in this.Keys)
			{
				object value = this[text];
				stringBuilder.Append(text + " => " + this.ValueToString(value)).Append(Environment.NewLine);
			}
			return stringBuilder.ToString();
		}

		private string ValueToString(object value)
		{
			string result;
			if (value is ArrayList)
			{
				string text = "ArrayList :: ";
				foreach (object current in ((ArrayList)value))
				{
					if (!text.EndsWith(", "))
					{
						text += ", ";
					}
					text = text + "[" + current.ToString() + "]";
				}
				result = text;
			}
			else
			{
				result = value.ToString();
			}
			return result;
		}

		public string GetString(string key)
		{
			return this.GetString(key, null);
		}

		public string GetString(string key, string defaultValue)
		{
			object obj = this[key];
			string result;
			if (obj is string)
			{
				result = (string)obj;
			}
			else if (obj == null)
			{
				if (this.defaults != null)
				{
					result = this.defaults.GetString(key, defaultValue);
				}
				else
				{
					result = defaultValue;
				}
			}
			else
			{
				if (!(obj is ArrayList))
				{
					throw new InvalidCastException('\'' + key + "' doesn't map to a String object");
				}
				result = (string)((ArrayList)obj)[0];
			}
			return result;
		}

		public Hashtable GetProperties(string key)
		{
			return this.GetProperties(key, new Hashtable());
		}

		public Hashtable GetProperties(string key, Hashtable defaults)
		{
			string[] stringArray = this.GetStringArray(key);
			Hashtable hashtable = new Hashtable(defaults);
			for (int i = 0; i < stringArray.Length; i++)
			{
				string text = stringArray[i];
				int num = text.IndexOf('=');
				if (num <= 0)
				{
					throw new ArgumentException('\'' + text + "' does not contain an equals sign");
				}
				string key2 = text.Substring(0, num).Trim();
				string newValue = text.Substring(num + 1).Trim();
				CollectionsUtil.PutElement(hashtable, key2, newValue);
			}
			return hashtable;
		}

		public string[] GetStringArray(string key)
		{
			object obj = this[key];
			ArrayList arrayList;
			string[] result;
			if (obj is string)
			{
				arrayList = new ArrayList(1);
				arrayList.Add(obj);
			}
			else if (obj is ArrayList)
			{
				arrayList = (ArrayList)obj;
			}
			else
			{
				if (obj != null)
				{
					throw new InvalidCastException('\'' + key + "' doesn't map to a String/Vector object");
				}
				if (this.defaults != null)
				{
					result = this.defaults.GetStringArray(key);
					return result;
				}
				result = new string[0];
				return result;
			}
			string[] array = new string[arrayList.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (string)arrayList[i];
			}
			result = array;
			return result;
		}

		public ArrayList GetVector(string key)
		{
			return this.GetVector(key, null);
		}

		public ArrayList GetVector(string key, ArrayList defaultValue)
		{
			object obj = this[key];
			ArrayList result;
			if (obj is ArrayList)
			{
				result = (ArrayList)obj;
			}
			else if (obj is string)
			{
				ArrayList arrayList = new ArrayList(1);
				arrayList.Add((string)obj);
				CollectionsUtil.PutElement(this, key, arrayList);
				result = arrayList;
			}
			else
			{
				if (obj != null)
				{
					throw new InvalidCastException('\'' + key + "' doesn't map to a Vector object");
				}
				if (this.defaults != null)
				{
					result = this.defaults.GetVector(key, defaultValue);
				}
				else
				{
					result = ((defaultValue == null) ? new ArrayList() : defaultValue);
				}
			}
			return result;
		}

		public bool GetBoolean(string key)
		{
			bool boolean = this.GetBoolean(key, ExtendedProperties.DEFAULT_BOOLEAN);
			//if (boolean != null)
			{
				return boolean;
			}
			throw new Exception('\'' + key + "' doesn't map to an existing object");
		}

		public bool GetBoolean(string key, bool defaultValue)
		{
			object obj = this[key];
			bool result;
			if (obj is bool)
			{
				result = (bool)obj;
			}
			else if (obj is string)
			{
				string text = this.TestBoolean((string)obj);
				bool flag = text.ToUpper().Equals("TRUE");
				CollectionsUtil.PutElement(this, key, flag);
				result = flag;
			}
			else
			{
				if (obj != null)
				{
					throw new InvalidCastException('\'' + key + "' doesn't map to a Boolean object");
				}
				if (this.defaults != null)
				{
					result = this.defaults.GetBoolean(key, defaultValue);
				}
				else
				{
					result = defaultValue;
				}
			}
			return result;
		}

		public string TestBoolean(string value_)
		{
			string text = value_.ToLower();
			string result;
			if (text.Equals("true") || text.Equals("on") || text.Equals("yes"))
			{
				result = "true";
			}
			else if (text.Equals("false") || text.Equals("off") || text.Equals("no"))
			{
				result = "false";
			}
			else
			{
				result = null;
			}
			return result;
		}

		public sbyte GetByte(string key)
		{
			if (this.ContainsKey(key))
			{
				byte @byte = this.GetByte(key, ExtendedProperties.DEFAULT_BYTE);
				return (sbyte)@byte;
			}
			throw new Exception('\'' + key + " doesn't map to an existing object");
		}

		public sbyte GetByte(string key, sbyte defaultValue)
		{
			return this.GetByte(key, defaultValue);
		}

		public byte GetByte(string key, byte defaultValue)
		{
			object obj = this[key];
			byte result;
			if (obj is byte)
			{
				result = (byte)obj;
			}
			else if (obj is string)
			{
				byte b = byte.Parse((string)obj);
				CollectionsUtil.PutElement(this, key, b);
				result = b;
			}
			else
			{
				if (obj != null)
				{
					throw new InvalidCastException('\'' + key + "' doesn't map to a Byte object");
				}
				if (this.defaults != null)
				{
					result = this.defaults.GetByte(key, defaultValue);
				}
				else
				{
					result = defaultValue;
				}
			}
			return result;
		}

		public short GetShort(string key)
		{
			short @short = this.GetShort(key, ExtendedProperties.DEFAULT_INT16);
			//if (@short != null)
			{
				return @short;
			}
			throw new Exception('\'' + key + "' doesn't map to an existing object");
		}

		public short GetShort(string key, short defaultValue)
		{
			object obj = this[key];
			short result;
			if (obj is short)
			{
				result = (short)obj;
			}
			else if (obj is string)
			{
				short num = short.Parse((string)obj);
				CollectionsUtil.PutElement(this, key, num);
				result = num;
			}
			else
			{
				if (obj != null)
				{
					throw new InvalidCastException('\'' + key + "' doesn't map to a Short object");
				}
				if (this.defaults != null)
				{
					result = this.defaults.GetShort(key, defaultValue);
				}
				else
				{
					result = defaultValue;
				}
			}
			return result;
		}

		public int GetInt(string name)
		{
			return this.GetInteger(name);
		}

		public int GetInt(string name, int def)
		{
			return this.GetInteger(name, def);
		}

		public int GetInteger(string key)
		{
			int integer = this.GetInteger(key, ExtendedProperties.DEFAULT_INT32);
			//if (integer != null)
			{
				return integer;
			}
			throw new Exception('\'' + key + "' doesn't map to an existing object");
		}

		public int GetInteger(string key, int defaultValue)
		{
			object obj = this[key];
			int result;
			if (obj is int)
			{
				result = (int)obj;
			}
			else if (obj is string)
			{
				int num = int.Parse((string)obj);
				CollectionsUtil.PutElement(this, key, num);
				result = num;
			}
			else
			{
				if (obj != null)
				{
					throw new InvalidCastException('\'' + key + "' doesn't map to a Integer object");
				}
				if (this.defaults != null)
				{
					result = this.defaults.GetInteger(key, defaultValue);
				}
				else
				{
					result = defaultValue;
				}
			}
			return result;
		}

		public long GetLong(string key)
		{
			long @long = this.GetLong(key, ExtendedProperties.DEFAULT_INT64);
			//if (@long != null)
			//{
				return @long;
			//}
			throw new Exception('\'' + key + "' doesn't map to an existing object");
		}

		public long GetLong(string key, long defaultValue)
		{
			object obj = this[key];
			long result;
			if (obj is long)
			{
				result = (long)obj;
			}
			else if (obj is string)
			{
				long num = long.Parse((string)obj);
				CollectionsUtil.PutElement(this, key, num);
				result = num;
			}
			else
			{
				if (obj != null)
				{
					throw new InvalidCastException('\'' + key + "' doesn't map to a Long object");
				}
				if (this.defaults != null)
				{
					result = this.defaults.GetLong(key, defaultValue);
				}
				else
				{
					result = defaultValue;
				}
			}
			return result;
		}

		public float GetFloat(string key)
		{
			float @float = this.GetFloat(key, ExtendedProperties.DEFAULT_SINGLE);
			//if (@float != null)
			{
				return @float;
			}
			throw new Exception('\'' + key + "' doesn't map to an existing object");
		}

		public float GetFloat(string key, float defaultValue)
		{
			object obj = this[key];
			float result;
			if (obj is float)
			{
				result = (float)obj;
			}
			else if (obj is string)
			{
				float num = float.Parse((string)obj);
				CollectionsUtil.PutElement(this, key, num);
				result = num;
			}
			else
			{
				if (obj != null)
				{
					throw new InvalidCastException('\'' + key + "' doesn't map to a Float object");
				}
				if (this.defaults != null)
				{
					result = this.defaults.GetFloat(key, defaultValue);
				}
				else
				{
					result = defaultValue;
				}
			}
			return result;
		}

		public double GetDouble(string key)
		{
			double @double = this.GetDouble(key, ExtendedProperties.DEFAULT_DOUBLE);
			//if (@double != null)
			{
				return @double;
			}
			throw new Exception('\'' + key + "' doesn't map to an existing object");
		}

		public double GetDouble(string key, double defaultValue)
		{
			object obj = this[key];
			double result;
			if (obj is double)
			{
				result = (double)obj;
			}
			else if (obj is string)
			{
				double num = double.Parse((string)obj);
				CollectionsUtil.PutElement(this, key, num);
				result = num;
			}
			else
			{
				if (obj != null)
				{
					throw new InvalidCastException('\'' + key + "' doesn't map to a Double object");
				}
				if (this.defaults != null)
				{
					result = this.defaults.GetDouble(key, defaultValue);
				}
				else
				{
					result = defaultValue;
				}
			}
			return result;
		}

		public static ExtendedProperties ConvertProperties(ExtendedProperties p)
		{
			ExtendedProperties extendedProperties = new ExtendedProperties();
			foreach (string key in p.Keys)
			{
				object obj = p.GetProperty(key);
				if (obj is string)
				{
					obj = obj.ToString().Replace(",", "\\,");
				}
				extendedProperties.SetProperty(key, obj);
			}
			return extendedProperties;
		}
	}
}
