using System;
using System.Collections;

namespace Commons.Collections
{
	[Serializable]
	public class LRUMap : IDictionary, ICollection, IEnumerable
	{
		[Serializable]
		private class SyncLRUMap : LRUMap, IDictionary, ICollection, IEnumerable
		{
			protected LRUMap _table;

			public override int Count
			{
				get
				{
					return this._table.Count;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return this._table.IsReadOnly;
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					return this._table.IsFixedSize;
				}
			}

			public override bool IsSynchronized
			{
				get
				{
					return true;
				}
			}

			public override object this[object key]
			{
				get
				{
					return this._table[key];
				}
				set
				{
					lock (this._table.SyncRoot)
					{
						this._table[key] = value;
					}
				}
			}

			public override object SyncRoot
			{
				get
				{
					return this._table.SyncRoot;
				}
			}

			public override ICollection Keys
			{
				get
				{
					ICollection keys;
					lock (this._table.SyncRoot)
					{
						keys = this._table.Keys;
					}
					return keys;
				}
			}

			public override ICollection Values
			{
				get
				{
					ICollection values;
					lock (this._table.SyncRoot)
					{
						values = this._table.Values;
					}
					return values;
				}
			}

			internal SyncLRUMap(LRUMap table)
			{
				this._table = table;
			}

			public override void Add(object key, object value)
			{
				lock (this._table.SyncRoot)
				{
					this._table.Add(key, value);
				}
			}

			public override void Clear()
			{
				lock (this._table.SyncRoot)
				{
					this._table.Clear();
				}
			}

			public override bool Contains(object key)
			{
				return this._table.Contains(key);
			}

			public override void CopyTo(Array array, int arrayIndex)
			{
				this._table.CopyTo(array, arrayIndex);
			}

			IDictionaryEnumerator IDictionary.GetEnumerator()
			{
				return ((IDictionary)this._table).GetEnumerator();
			}

			public override void Remove(object key)
			{
				lock (this._table.SyncRoot)
				{
					this._table.Remove(key);
				}
			}
		}

		protected internal const int DEFAULT_MAX_SIZE = 100;

		private Hashtable objectTable = new Hashtable();

		private ArrayList objectList = new ArrayList();

		[NonSerialized]
		private int maxSize;

		public virtual int Count
		{
			get
			{
				return this.objectList.Count;
			}
		}

		public virtual bool IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public int MaxSize
		{
			get
			{
				return this.maxSize;
			}
		}

		public virtual object this[object key]
		{
			get
			{
				this.MoveToMRU(key);
				return this.objectTable[key];
			}
			set
			{
				if (this.objectTable.Contains(key))
				{
					this.Remove(key);
				}
				this.Add(key, value);
			}
		}

		public virtual ICollection Keys
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

		public virtual ICollection Values
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

		public virtual object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public LRUMap() : this(100)
		{
		}

		public LRUMap(int maxSize)
		{
			this.maxSize = maxSize;
		}

		public virtual void Add(object key, object value)
		{
			if (this.objectList.Count == this.maxSize)
			{
				this.RemoveLRU();
			}
			this.objectTable.Add(key, value);
			this.objectList.Insert(0, new DictionaryEntry(key, value));
		}

		public virtual void Clear()
		{
			this.objectTable.Clear();
			this.objectList.Clear();
		}

		public virtual bool Contains(object key)
		{
			return this.objectTable.Contains(key);
		}

		public virtual void CopyTo(Array array, int idx)
		{
			this.objectTable.CopyTo(array, idx);
		}

		public virtual void Remove(object key)
		{
			this.objectTable.Remove(key);
			this.objectList.RemoveAt(this.IndexOf(key));
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new KeyedListEnumerator(this.objectList);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new KeyedListEnumerator(this.objectList);
		}

		public void AddAll(IDictionary dictionary)
		{
			foreach (DictionaryEntry dictionaryEntry in dictionary)
			{
				this.Add(dictionaryEntry.Key, dictionaryEntry.Value);
			}
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

		private void RemoveLRU()
		{
			object key = ((DictionaryEntry)this.objectList[this.objectList.Count - 1]).Key;
			this.objectTable.Remove(key);
			this.objectList.RemoveAt(this.objectList.Count - 1);
		}

		private void MoveToMRU(object key)
		{
			int num = this.IndexOf(key);
			if (num > 0)
			{
				object value = this.objectList[num];
				this.objectList.RemoveAt(num);
				this.objectList.Insert(0, value);
			}
		}

		public static LRUMap Synchronized(LRUMap table)
		{
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			return new LRUMap.SyncLRUMap(table);
		}
	}
}
