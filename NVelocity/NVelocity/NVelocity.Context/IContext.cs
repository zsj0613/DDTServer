using System;

namespace NVelocity.Context
{
	public interface IContext
	{
		int Count
		{
			get;
		}

		object[] Keys
		{
			get;
		}

		object Put(string key, object value);

		object Get(string key);

		bool ContainsKey(object key);

		object Remove(object key);
	}
}
