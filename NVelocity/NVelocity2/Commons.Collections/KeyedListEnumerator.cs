using System;
using System.Collections;

namespace Commons.Collections
{
	public class KeyedListEnumerator : IDictionaryEnumerator, IEnumerator
	{
		private int index = -1;

		private ArrayList objs;

		public object Current
		{
			get
			{
				if (this.index < 0 || this.index >= this.objs.Count)
				{
					throw new InvalidOperationException();
				}
				return this.objs[this.index];
			}
		}

		public DictionaryEntry Entry
		{
			get
			{
				return (DictionaryEntry)this.Current;
			}
		}

		public object Key
		{
			get
			{
				return this.Entry.Key;
			}
		}

		public object Value
		{
			get
			{
				return this.Entry.Value;
			}
		}

		internal KeyedListEnumerator(ArrayList list)
		{
			this.objs = list;
		}

		public bool MoveNext()
		{
			this.index++;
			return this.index < this.objs.Count;
		}

		public void Reset()
		{
			this.index = -1;
		}
	}
}
