using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace NVelocity
{
	public class SupportClass
	{
		public class TransactionManager
		{
			public class ConnectionHashTable : Hashtable
			{
				private class ConnectionProperties
				{
					public bool AutoCommit;

					public OleDbTransaction Transaction;

					public IsolationLevel TransactionLevel;
				}

				public OleDbCommand CreateStatement(OleDbConnection connection)
				{
					OleDbCommand oleDbCommand = connection.CreateCommand();
					if (this[connection] != null)
					{
						SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties connectionProperties = (SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties)this[connection];
						OleDbTransaction transaction = connectionProperties.Transaction;
						oleDbCommand.Transaction = transaction;
					}
					else
					{
						SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties connectionProperties2 = new SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties();
						connectionProperties2.AutoCommit = false;
						connectionProperties2.TransactionLevel = (IsolationLevel)0;
						oleDbCommand.Transaction = connectionProperties2.Transaction;
						this.Add(connection, connectionProperties2);
					}
					return oleDbCommand;
				}

				public void Commit(OleDbConnection connection)
				{
					if (this[connection] != null && ((SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties)this[connection]).AutoCommit)
					{
						OleDbTransaction transaction = ((SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties)this[connection]).Transaction;
						transaction.Commit();
					}
				}

				public void RollBack(OleDbConnection connection)
				{
					if (this[connection] != null && ((SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties)this[connection]).AutoCommit)
					{
						OleDbTransaction transaction = ((SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties)this[connection]).Transaction;
						transaction.Rollback();
					}
				}

				public void SetAutoCommit(OleDbConnection connection, bool boolean)
				{
					if (this[connection] != null)
					{
						SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties connectionProperties = (SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties)this[connection];
						connectionProperties.AutoCommit = boolean;
						if (!boolean)
						{
							OleDbTransaction oleDbTransaction = connectionProperties.Transaction;
							if (connectionProperties.TransactionLevel == (IsolationLevel)0)
							{
								oleDbTransaction = connection.BeginTransaction();
							}
							else
							{
								oleDbTransaction = connection.BeginTransaction(connectionProperties.TransactionLevel);
							}
						}
					}
					else
					{
						SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties connectionProperties2 = new SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties();
						connectionProperties2.AutoCommit = boolean;
						connectionProperties2.TransactionLevel = (IsolationLevel)0;
						if (boolean)
						{
							connectionProperties2.Transaction = connection.BeginTransaction();
						}
						this.Add(connection, connectionProperties2);
					}
				}

				public OleDbCommand PrepareStatement(OleDbConnection connection, string sql)
				{
					OleDbCommand oleDbCommand = this.CreateStatement(connection);
					oleDbCommand.CommandText = sql;
					return oleDbCommand;
				}

				public OleDbCommand PrepareCall(OleDbConnection connection, string sql)
				{
					OleDbCommand oleDbCommand = this.CreateStatement(connection);
					oleDbCommand.CommandText = sql;
					return oleDbCommand;
				}

				public void SetTransactionIsolation(OleDbConnection connection, int level)
				{
					if (this[connection] != null)
					{
						SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties connectionProperties = (SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties)this[connection];
						connectionProperties.TransactionLevel = (IsolationLevel)level;
					}
					else
					{
						this.Add(connection, new SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties
						{
							AutoCommit = false,
							TransactionLevel = (IsolationLevel)level
						});
					}
				}

				public int GetTransactionIsolation(OleDbConnection connection)
				{
					int result;
					if (this[connection] != null)
					{
						SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties connectionProperties = (SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties)this[connection];
						if (connectionProperties.TransactionLevel != (IsolationLevel)0)
						{
							result = (int)connectionProperties.TransactionLevel;
						}
						else
						{
							result = 2;
						}
					}
					else
					{
						result = 2;
					}
					return result;
				}

				public bool GetAutoCommit(OleDbConnection connection)
				{
					return this[connection] != null && ((SupportClass.TransactionManager.ConnectionHashTable.ConnectionProperties)this[connection]).AutoCommit;
				}
			}

			public static SupportClass.TransactionManager.ConnectionHashTable manager = new SupportClass.TransactionManager.ConnectionHashTable();
		}

		public class Tokenizer
		{
			private ArrayList elements;

			private string source;

			private string delimiters = " \t\n\r";

			public int Count
			{
				get
				{
					return this.elements.Count;
				}
			}

			public Tokenizer(string source)
			{
				this.elements = new ArrayList();
				this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
				this.RemoveEmptyStrings();
				this.source = source;
			}

			public Tokenizer(string source, string delimiters)
			{
				this.elements = new ArrayList();
				this.delimiters = delimiters;
				this.elements.AddRange(source.Split(this.delimiters.ToCharArray()));
				this.RemoveEmptyStrings();
				this.source = source;
			}

			public bool HasMoreTokens()
			{
				return this.elements.Count > 0;
			}

			public string NextToken()
			{
				if (this.source == "")
				{
					throw new System.Exception();
				}
				this.elements = new ArrayList();
				this.elements.AddRange(this.source.Split(this.delimiters.ToCharArray()));
				this.RemoveEmptyStrings();
				string text = (string)this.elements[0];
				this.elements.RemoveAt(0);
				this.source = this.source.Replace(text, "");
				this.source = this.source.TrimStart(this.delimiters.ToCharArray());
				return text;
			}

			public string NextToken(string delimiters)
			{
				this.delimiters = delimiters;
				return this.NextToken();
			}

			private void RemoveEmptyStrings()
			{
				for (int i = 0; i < this.elements.Count; i++)
				{
					if ((string)this.elements[i] == "")
					{
						this.elements.RemoveAt(i);
						i--;
					}
				}
			}
		}

		public class TextNumberFormat
		{
			private enum formatTypes
			{
				General,
				Number,
				Currency,
				Percent
			}

			private NumberFormatInfo numberFormat;

			private int numberFormatType;

			private bool groupingActivated;

			private string separator;

			private int digits;

			public bool GroupingUsed
			{
				get
				{
					return this.groupingActivated;
				}
				set
				{
					this.groupingActivated = value;
				}
			}

			public int Digits
			{
				get
				{
					return this.digits;
				}
				set
				{
					this.digits = value;
				}
			}

			public TextNumberFormat()
			{
				this.numberFormat = new NumberFormatInfo();
				this.numberFormatType = 0;
				this.groupingActivated = true;
				this.separator = this.GetSeparator(0);
				this.digits = 3;
			}

			private TextNumberFormat(SupportClass.TextNumberFormat.formatTypes theType, int digits)
			{
				this.numberFormat = NumberFormatInfo.CurrentInfo;
				this.numberFormatType = (int)theType;
				this.groupingActivated = true;
				this.separator = this.GetSeparator((int)theType);
				this.digits = digits;
			}

			private TextNumberFormat(SupportClass.TextNumberFormat.formatTypes theType, CultureInfo cultureNumberFormat, int digits)
			{
				this.numberFormat = cultureNumberFormat.NumberFormat;
				this.numberFormatType = (int)theType;
				this.groupingActivated = true;
				this.separator = this.GetSeparator((int)theType);
				this.digits = digits;
			}

			public static SupportClass.TextNumberFormat getTextNumberInstance()
			{
				return new SupportClass.TextNumberFormat(SupportClass.TextNumberFormat.formatTypes.Number, 3);
			}

			public static SupportClass.TextNumberFormat getTextNumberCurrencyInstance()
			{
				return new SupportClass.TextNumberFormat(SupportClass.TextNumberFormat.formatTypes.Currency, 3);
			}

			public static SupportClass.TextNumberFormat getTextNumberPercentInstance()
			{
				return new SupportClass.TextNumberFormat(SupportClass.TextNumberFormat.formatTypes.Percent, 3);
			}

			public static SupportClass.TextNumberFormat getTextNumberInstance(CultureInfo culture)
			{
				return new SupportClass.TextNumberFormat(SupportClass.TextNumberFormat.formatTypes.Number, culture, 3);
			}

			public static SupportClass.TextNumberFormat getTextNumberCurrencyInstance(CultureInfo culture)
			{
				return new SupportClass.TextNumberFormat(SupportClass.TextNumberFormat.formatTypes.Currency, culture, 3);
			}

			public static SupportClass.TextNumberFormat getTextNumberPercentInstance(CultureInfo culture)
			{
				return new SupportClass.TextNumberFormat(SupportClass.TextNumberFormat.formatTypes.Percent, culture, 3);
			}

			public object Clone()
			{
				return this;
			}

			public override bool Equals(object textNumberObject)
			{
				return object.Equals(this, textNumberObject);
			}

			public string FormatDouble(double number)
			{
				string result;
				if (this.groupingActivated)
				{
					result = number.ToString(this.GetCurrentFormatString() + this.digits, this.numberFormat);
				}
				else
				{
					result = number.ToString(this.GetCurrentFormatString() + this.digits, this.numberFormat).Replace(this.separator, "");
				}
				return result;
			}

			public string FormatLong(long number)
			{
				string result;
				if (this.groupingActivated)
				{
					result = number.ToString(this.GetCurrentFormatString() + this.digits, this.numberFormat);
				}
				else
				{
					result = number.ToString(this.GetCurrentFormatString() + this.digits, this.numberFormat).Replace(this.separator, "");
				}
				return result;
			}

			public static CultureInfo[] GetAvailableCultures()
			{
				return CultureInfo.GetCultures(CultureTypes.AllCultures);
			}

			public override int GetHashCode()
			{
				return this.GetHashCode();
			}

			private string GetCurrentFormatString()
			{
				string result = "n";
				switch (this.numberFormatType)
				{
				case 0:
					result = "n" + this.numberFormat.NumberDecimalDigits;
					break;
				case 1:
					result = "n" + this.numberFormat.NumberDecimalDigits;
					break;
				case 2:
					result = "c";
					break;
				case 3:
					result = "p";
					break;
				}
				return result;
			}

			private string GetSeparator(int numberFormatType)
			{
				string result = " ";
				switch (numberFormatType)
				{
				case 0:
					result = this.numberFormat.NumberGroupSeparator;
					break;
				case 1:
					result = this.numberFormat.NumberGroupSeparator;
					break;
				case 2:
					result = this.numberFormat.CurrencyGroupSeparator;
					break;
				case 3:
					result = this.numberFormat.PercentGroupSeparator;
					break;
				}
				return result;
			}
		}

		public class DateTimeFormatManager
		{
			public class DateTimeFormatHashTable : Hashtable
			{
				private class DateTimeFormatProperties
				{
					public string DateFormatPattern = "d-MMM-yy";

					public string TimeFormatPattern = "h:mm:ss tt";
				}

				public void SetDateFormatPattern(DateTimeFormatInfo format, string newPattern)
				{
					if (this[format] != null)
					{
						((SupportClass.DateTimeFormatManager.DateTimeFormatHashTable.DateTimeFormatProperties)this[format]).DateFormatPattern = newPattern;
					}
					else
					{
						this.Add(format, new SupportClass.DateTimeFormatManager.DateTimeFormatHashTable.DateTimeFormatProperties
						{
							DateFormatPattern = newPattern
						});
					}
				}

				public string GetDateFormatPattern(DateTimeFormatInfo format)
				{
					string result;
					if (this[format] == null)
					{
						result = "d-MMM-yy";
					}
					else
					{
						result = ((SupportClass.DateTimeFormatManager.DateTimeFormatHashTable.DateTimeFormatProperties)this[format]).DateFormatPattern;
					}
					return result;
				}

				public void SetTimeFormatPattern(DateTimeFormatInfo format, string newPattern)
				{
					if (this[format] != null)
					{
						((SupportClass.DateTimeFormatManager.DateTimeFormatHashTable.DateTimeFormatProperties)this[format]).TimeFormatPattern = newPattern;
					}
					else
					{
						this.Add(format, new SupportClass.DateTimeFormatManager.DateTimeFormatHashTable.DateTimeFormatProperties
						{
							TimeFormatPattern = newPattern
						});
					}
				}

				public string GetTimeFormatPattern(DateTimeFormatInfo format)
				{
					string result;
					if (this[format] == null)
					{
						result = "h:mm:ss tt";
					}
					else
					{
						result = ((SupportClass.DateTimeFormatManager.DateTimeFormatHashTable.DateTimeFormatProperties)this[format]).TimeFormatPattern;
					}
					return result;
				}
			}

			public static SupportClass.DateTimeFormatManager.DateTimeFormatHashTable manager = new SupportClass.DateTimeFormatManager.DateTimeFormatHashTable();
		}

		public static sbyte[] ToSByteArray(byte[] byteArray)
		{
			sbyte[] array = new sbyte[byteArray.Length];
			for (int i = 0; i < byteArray.Length; i++)
			{
				array[i] = (sbyte)byteArray[i];
			}
			return array;
		}

		public static byte[] ToByteArray(sbyte[] sbyteArray)
		{
			byte[] array = new byte[sbyteArray.Length];
			for (int i = 0; i < sbyteArray.Length; i++)
			{
				array[i] = (byte)sbyteArray[i];
			}
			return array;
		}

		public static object PutElement(Hashtable hashTable, object key, object newValue)
		{
			object result = hashTable[key];
			hashTable[key] = newValue;
			return result;
		}

		public static long FileLength(FileInfo file)
		{
			long result;
			if (Directory.Exists(file.FullName))
			{
				result = 0L;
			}
			else
			{
				result = file.Length;
			}
			return result;
		}

		public static void WriteStackTrace(System.Exception throwable, TextWriter stream)
		{
			stream.Write(throwable.StackTrace);
			stream.Flush();
		}

		public static string FormatDateTime(DateTimeFormatInfo format, DateTime date)
		{
			string timeFormatPattern = SupportClass.DateTimeFormatManager.manager.GetTimeFormatPattern(format);
			string dateFormatPattern = SupportClass.DateTimeFormatManager.manager.GetDateFormatPattern(format);
			return date.ToString(dateFormatPattern + " " + timeFormatPattern, format);
		}

		public static DateTimeFormatInfo GetDateTimeFormatInstance(int dateStyle, int timeStyle, CultureInfo culture)
		{
			DateTimeFormatInfo dateTimeFormat = culture.DateTimeFormat;
			switch (timeStyle)
			{
			case -1:
				SupportClass.DateTimeFormatManager.manager.SetTimeFormatPattern(dateTimeFormat, "");
				break;
			case 0:
				SupportClass.DateTimeFormatManager.manager.SetTimeFormatPattern(dateTimeFormat, "h:mm:ss 'o clock' tt zzz");
				break;
			case 1:
				SupportClass.DateTimeFormatManager.manager.SetTimeFormatPattern(dateTimeFormat, "h:mm:ss tt zzz");
				break;
			case 2:
				SupportClass.DateTimeFormatManager.manager.SetTimeFormatPattern(dateTimeFormat, "h:mm:ss tt");
				break;
			case 3:
				SupportClass.DateTimeFormatManager.manager.SetTimeFormatPattern(dateTimeFormat, "h:mm tt");
				break;
			}
			switch (dateStyle)
			{
			case -1:
				SupportClass.DateTimeFormatManager.manager.SetDateFormatPattern(dateTimeFormat, "");
				break;
			case 0:
				SupportClass.DateTimeFormatManager.manager.SetDateFormatPattern(dateTimeFormat, "dddd, MMMM dd%, yyy");
				break;
			case 1:
				SupportClass.DateTimeFormatManager.manager.SetDateFormatPattern(dateTimeFormat, "MMMM dd%, yyy");
				break;
			case 2:
				SupportClass.DateTimeFormatManager.manager.SetDateFormatPattern(dateTimeFormat, "d-MMM-yy");
				break;
			case 3:
				SupportClass.DateTimeFormatManager.manager.SetDateFormatPattern(dateTimeFormat, "M/dd/yy");
				break;
			}
			return dateTimeFormat;
		}

		public static object CreateNewInstance(Type classType)
		{
			ConstructorInfo[] constructors = classType.GetConstructors();
			object result;
			if (constructors.Length == 0)
			{
				result = null;
			}
			else
			{
				ParameterInfo[] parameters = constructors[0].GetParameters();
				int num = parameters.Length;
				Type[] array = new Type[num];
				for (int i = 0; i < num; i++)
				{
					array[i] = parameters[i].ParameterType;
				}
				result = classType.GetConstructor(array).Invoke(new object[0]);
			}
			return result;
		}
	}
}
