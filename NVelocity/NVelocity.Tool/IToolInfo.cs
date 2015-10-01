using System;

namespace NVelocity.Tool
{
	public interface IToolInfo
	{
		string Key
		{
			get;
		}

		string Classname
		{
			get;
		}

		object getInstance(object initData);
	}
}
