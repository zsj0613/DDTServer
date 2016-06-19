using System;
using System.Collections;

namespace Commons.Collections
{
	public class CollectionsUtil
	{
		public static object PutElement(IDictionary hashTable, object key, object newValue)
		{
			object result = hashTable[key];
			hashTable[key] = newValue;
			return result;
		}
	}
}
