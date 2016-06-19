using System;
using System.Collections;

namespace Commons.Collections
{
	[Serializable]
	public class KeyedList : IDictionary, ICollection, IEnumerable, IOrderedDictionary
	{
		private Hashtable objectTable = new Hashtable();

		private ArrayList objectList = new ArrayList();

		public int Count
		{
			get
			{
				return this.objectList.Count;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object this[int idx]
		{
			get
			{
				return ((DictionaryEntry)this.objectList[idx]).Value;
			}
			set
			{
				if (idx < 0 || idx >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				object key = ((DictionaryEntry)this.objectList[idx]).Key;
				this.objectList[idx] = new DictionaryEntry(key, value);
				this.objectTable[key] = value;
			}
		}

		public object this[object key]
		{
			get
			{
				return this.objectTable[key];
			}
			set
			{
				if (this.objectTable.Contains(key))
				{
					this.objectTable[key] = value;
					this.objectTable[this.IndexOf(key)] = new DictionaryEntry(key, value);
				}
				else
				{
					this.Add(key, value);
				}
			}
		}

		public ICollection Keys
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < this.objectList.Count; i++)
				{
					arrayList.Add(((DictionaryEntry)this.objectList[i]).Key);
				}
				return arrayList;
			}
		}

		public ICollection Values
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < this.objectList.Count; i++)
				{
					arrayList.Add(((DictionaryEntry)this.objectList[i]).Value);
				}
				return arrayList;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public void Add(object key, object value)
		{
			this.objectTable.Add(key, value);
			this.objectList.Add(new DictionaryEntry(key, value));
		}

		public void Clear()
		{
			this.objectTable.Clear();
			this.objectList.Clear();
		}

		public bool Contains(object key)
		{
			return this.objectTable.Contains(key);
		}

		public void CopyTo(Array array, int idx)
		{
			this.objectTable.CopyTo(array, idx);
		}

		public void Insert(int idx, object key, object value)
		{
			if (idx > this.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this.objectTable.Add(key, value);
			this.objectList.Insert(idx, new DictionaryEntry(key, value));
		}

		public void Remove(object key)
		{
			this.objectTable.Remove(key);
			this.objectList.RemoveAt(this.IndexOf(key));
		}

		public void RemoveAt(int idx)
		{
			if (idx >= this.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this.objectTable.Remove(((DictionaryEntry)this.objectList[idx]).Key);
			this.objectList.RemoveAt(idx);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new KeyedListEnumerator(this.objectList);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new KeyedListEnumerator(this.objectList);
		}

		private int IndexOf(object key)
		{
			int result;
			for (int i = 0; i < this.objectList.Count; i++)
			{
				if (((DictionaryEntry)this.objectList[i]).Key.Equals(key))
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
		}
	}
}
