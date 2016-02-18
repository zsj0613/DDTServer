using System;

namespace NVelocity.App.Events
{
	public class NullSetEventArgs : EventArgs
	{
		private bool shouldLog = true;

		private string lhs;

		private string rhs;

		public string LHS
		{
			get
			{
				return this.lhs;
			}
		}

		public string RHS
		{
			get
			{
				return this.rhs;
			}
		}

		public bool ShouldLog
		{
			get
			{
				return this.shouldLog;
			}
			set
			{
				this.shouldLog = value;
			}
		}

		public NullSetEventArgs(string lhs, string rhs)
		{
			this.lhs = lhs;
			this.rhs = rhs;
		}
	}
}
