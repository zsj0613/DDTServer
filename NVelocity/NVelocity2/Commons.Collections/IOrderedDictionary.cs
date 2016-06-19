using System;

namespace Commons.Collections
{
	public interface IOrderedDictionary
	{
		void Insert(int index, object key, object value);

		void RemoveAt(int index);
	}
}
