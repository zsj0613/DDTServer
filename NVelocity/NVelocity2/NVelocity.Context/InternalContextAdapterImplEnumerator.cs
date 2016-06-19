using System;
using System.Collections;

namespace NVelocity.Context
{
	public class InternalContextAdapterImplEnumerator : IDictionaryEnumerator, IEnumerator
	{
		private int index = -1;

		private IContext ctx;

		private object[] keys;

		public object Key
		{
			get
			{
				return this.keys[this.index];
			}
		}

		public object Value
		{
			get
			{
				return this.ctx.Get(this.keys[this.index].ToString());
			}
		}

		public DictionaryEntry Entry
		{
			get
			{
				return new DictionaryEntry(this.Key, this.Value);
			}
		}

		public object Current
		{
			get
			{
				return this.Entry;
			}
		}

		public InternalContextAdapterImplEnumerator(IContext context)
		{
			this.ctx = context;
			this.keys = context.Keys;
		}

		public void Reset()
		{
			this.index = -1;
		}

		public bool MoveNext()
		{
			return ++this.index < this.keys.Length;
		}
	}
}
