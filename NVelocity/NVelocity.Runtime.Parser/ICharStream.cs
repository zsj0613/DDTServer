using System;

namespace NVelocity.Runtime.Parser
{
	public interface ICharStream
	{
		int Column
		{
			get;
		}

		int Line
		{
			get;
		}

		int EndColumn
		{
			get;
		}

		int EndLine
		{
			get;
		}

		int BeginColumn
		{
			get;
		}

		int BeginLine
		{
			get;
		}

		char ReadChar();

		void Backup(int amount);

		char BeginToken();

		string GetImage();

		char[] GetSuffix(int len);

		void Done();
	}
}
