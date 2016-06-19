using System;
using System.IO;

namespace NVelocity.Runtime.Parser
{
	public sealed class VelocityCharStream : ICharStream
	{
		public const bool staticFlag = false;

		internal int bufsize;

		internal int available;

		internal int tokenBegin;

		public int bufpos = -1;

		private int[] bufline;

		private int[] bufcolumn;

		private int column = 0;

		private int line = 1;

		private bool prevCharIsCR = false;

		private bool prevCharIsLF = false;

		private TextReader inputStream;

		private char[] buffer;

		private int maxNextCharInd = 0;

		private int inBuf = 0;

		public int Column
		{
			get
			{
				return this.bufcolumn[this.bufpos];
			}
		}

		public int Line
		{
			get
			{
				return this.bufline[this.bufpos];
			}
		}

		public int EndColumn
		{
			get
			{
				return this.bufcolumn[this.bufpos];
			}
		}

		public int EndLine
		{
			get
			{
				return this.bufline[this.bufpos];
			}
		}

		public int BeginColumn
		{
			get
			{
				return this.bufcolumn[this.tokenBegin];
			}
		}

		public int BeginLine
		{
			get
			{
				return this.bufline[this.tokenBegin];
			}
		}

		private void ExpandBuff(bool wrapAround)
		{
			char[] destinationArray = new char[this.bufsize + 2048];
			int[] destinationArray2 = new int[this.bufsize + 2048];
			int[] destinationArray3 = new int[this.bufsize + 2048];
			try
			{
				if (wrapAround)
				{
					Array.Copy(this.buffer, this.tokenBegin, destinationArray, 0, this.bufsize - this.tokenBegin);
					Array.Copy(this.buffer, 0, destinationArray, this.bufsize - this.tokenBegin, this.bufpos);
					this.buffer = destinationArray;
					Array.Copy(this.bufline, this.tokenBegin, destinationArray2, 0, this.bufsize - this.tokenBegin);
					Array.Copy(this.bufline, 0, destinationArray2, this.bufsize - this.tokenBegin, this.bufpos);
					this.bufline = destinationArray2;
					Array.Copy(this.bufcolumn, this.tokenBegin, destinationArray3, 0, this.bufsize - this.tokenBegin);
					Array.Copy(this.bufcolumn, 0, destinationArray3, this.bufsize - this.tokenBegin, this.bufpos);
					this.bufcolumn = destinationArray3;
					this.maxNextCharInd = (this.bufpos += this.bufsize - this.tokenBegin);
				}
				else
				{
					Array.Copy(this.buffer, this.tokenBegin, destinationArray, 0, this.bufsize - this.tokenBegin);
					this.buffer = destinationArray;
					Array.Copy(this.bufline, this.tokenBegin, destinationArray2, 0, this.bufsize - this.tokenBegin);
					this.bufline = destinationArray2;
					Array.Copy(this.bufcolumn, this.tokenBegin, destinationArray3, 0, this.bufsize - this.tokenBegin);
					this.bufcolumn = destinationArray3;
					this.maxNextCharInd = (this.bufpos -= this.tokenBegin);
				}
			}
			catch (System.Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}
			this.bufsize += 2048;
			this.available = this.bufsize;
			this.tokenBegin = 0;
		}

		private void FillBuff()
		{
			if (this.maxNextCharInd == this.available)
			{
				if (this.available == this.bufsize)
				{
					if (this.tokenBegin > 2048)
					{
						this.bufpos = (this.maxNextCharInd = 0);
						this.available = this.tokenBegin;
					}
					else if (this.tokenBegin < 0)
					{
						this.bufpos = (this.maxNextCharInd = 0);
					}
					else
					{
						this.ExpandBuff(false);
					}
				}
				else if (this.available > this.tokenBegin)
				{
					this.available = this.bufsize;
				}
				else if (this.tokenBegin - this.available < 2048)
				{
					this.ExpandBuff(true);
				}
				else
				{
					this.available = this.tokenBegin;
				}
			}
			try
			{
				int num;
				try
				{
					num = this.inputStream.Read(this.buffer, this.maxNextCharInd, this.available - this.maxNextCharInd);
				}
				catch (System.Exception innerException)
				{
					throw new IOException("exception reading from inputStream", innerException);
				}
				if (num <= 0)
				{
					this.inputStream.Close();
					throw new IOException();
				}
				this.maxNextCharInd += num;
			}
			catch (IOException ex)
			{
				this.bufpos--;
				this.Backup(0);
				if (this.tokenBegin == -1)
				{
					this.tokenBegin = this.bufpos;
				}
				throw ex;
			}
		}

		public char BeginToken()
		{
			this.tokenBegin = -1;
			char result = this.ReadChar();
			this.tokenBegin = this.bufpos;
			return result;
		}

		private void UpdateLineColumn(char c)
		{
			this.column++;
			if (this.prevCharIsLF)
			{
				this.prevCharIsLF = false;
				this.line += (this.column = 1);
			}
			else if (this.prevCharIsCR)
			{
				this.prevCharIsCR = false;
				if (c == '\n')
				{
					this.prevCharIsLF = true;
				}
				else
				{
					this.line += (this.column = 1);
				}
			}
			switch (c)
			{
			case '\t':
				this.column--;
				this.column += 8 - (this.column & 7);
				break;
			case '\n':
				this.prevCharIsLF = true;
				break;
			case '\r':
				this.prevCharIsCR = true;
				break;
			}
			this.bufline[this.bufpos] = this.line;
			this.bufcolumn[this.bufpos] = this.column;
		}

		public char ReadChar()
		{
			char result;
			if (this.inBuf > 0)
			{
				this.inBuf--;
				result = this.buffer[(this.bufpos == this.bufsize - 1) ? (this.bufpos = 0) : (++this.bufpos)];
			}
			else
			{
				if (++this.bufpos >= this.maxNextCharInd)
				{
					this.FillBuff();
				}
				char c = this.buffer[this.bufpos];
				this.UpdateLineColumn(c);
				result = c;
			}
			return result;
		}

		public void Backup(int amount)
		{
			this.inBuf += amount;
			if ((this.bufpos -= amount) < 0)
			{
				this.bufpos += this.bufsize;
			}
		}

		public VelocityCharStream(TextReader dstream, int startline, int startcolumn, int buffersize)
		{
			this.inputStream = dstream;
			this.line = startline;
			this.column = startcolumn - 1;
			this.bufsize = buffersize;
			this.available = buffersize;
			this.buffer = new char[buffersize];
			this.bufline = new int[buffersize];
			this.bufcolumn = new int[buffersize];
		}

		public VelocityCharStream(TextReader dstream, int startline, int startcolumn) : this(dstream, startline, startcolumn, 4096)
		{
		}

		public void ReInit(TextReader dstream, int startline, int startcolumn, int buffersize)
		{
			this.inputStream = dstream;
			this.line = startline;
			this.column = startcolumn - 1;
			if (this.buffer == null || buffersize != this.buffer.Length)
			{
				this.bufsize = buffersize;
				this.available = buffersize;
				this.buffer = new char[buffersize];
				this.bufline = new int[buffersize];
				this.bufcolumn = new int[buffersize];
			}
			this.prevCharIsLF = (this.prevCharIsCR = false);
			this.tokenBegin = (this.inBuf = (this.maxNextCharInd = 0));
			this.bufpos = -1;
		}

		public void ReInit(TextReader dstream, int startline, int startcolumn)
		{
			this.ReInit(dstream, startline, startcolumn, 4096);
		}

		public string GetImage()
		{
			string result;
			if (this.bufpos >= this.tokenBegin)
			{
				int length = (this.bufpos - this.tokenBegin + 1 > this.buffer.Length) ? this.buffer.Length : (this.bufpos - this.tokenBegin + 1);
				result = new string(this.buffer, this.tokenBegin, length);
			}
			else
			{
				result = new string(this.buffer, this.tokenBegin, this.bufsize - this.tokenBegin) + new string(this.buffer, 0, this.bufpos + 1);
			}
			return result;
		}

		public char[] GetSuffix(int len)
		{
			char[] array = new char[len];
			if (this.bufpos + 1 >= len)
			{
				Array.Copy(this.buffer, this.bufpos - len + 1, array, 0, len);
			}
			else
			{
				Array.Copy(this.buffer, this.bufsize - (len - this.bufpos - 1), array, 0, len - this.bufpos - 1);
				Array.Copy(this.buffer, 0, array, len - this.bufpos - 1, this.bufpos + 1);
			}
			return array;
		}

		public void Done()
		{
			this.buffer = null;
			this.bufline = null;
			this.bufcolumn = null;
		}

		public void AdjustBeginLineColumn(int newLine, int newCol)
		{
			int num = this.tokenBegin;
			int num2;
			if (this.bufpos >= this.tokenBegin)
			{
				num2 = this.bufpos - this.tokenBegin + this.inBuf + 1;
			}
			else
			{
				num2 = this.bufsize - this.tokenBegin + this.bufpos + 1 + this.inBuf;
			}
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			while (num3 < num2 && this.bufline[num4 = num % this.bufsize] == this.bufline[num5 = ++num % this.bufsize])
			{
				this.bufline[num4] = newLine;
				int num7 = num6 + this.bufcolumn[num5] - this.bufcolumn[num4];
				this.bufcolumn[num4] = newCol + num6;
				num6 = num7;
				num3++;
			}
			if (num3 < num2)
			{
				this.bufline[num4] = newLine++;
				this.bufcolumn[num4] = newCol + num6;
				while (num3++ < num2)
				{
					if (this.bufline[num4 = num % this.bufsize] != this.bufline[++num % this.bufsize])
					{
						this.bufline[num4] = newLine++;
					}
					else
					{
						this.bufline[num4] = newLine;
					}
				}
			}
			this.line = this.bufline[num4];
			this.column = this.bufcolumn[num4];
		}
	}
}
