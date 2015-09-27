using System;
using System.Threading;
namespace Game.Base.Packets
{
	internal class EmptyAsyncResult : IAsyncResult
	{
		private object m_asyncState;
		public object AsyncState
		{
			get
			{
				return this.m_asyncState;
			}
		}
		public WaitHandle AsyncWaitHandle
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public bool CompletedSynchronously
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public bool IsCompleted
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		public EmptyAsyncResult(object state)
		{
			this.m_asyncState = state;
		}
	}
}
