using System;
namespace Game.Logic
{
	public class MacroDropInfo
	{
		public int SelfDropCount
		{
			get;
			set;
		}
		public int DropCount
		{
			get;
			set;
		}
		public int MaxDropCount
		{
			get;
			set;
		}
		public MacroDropInfo(int dropCount, int maxDropCount)
		{
			this.DropCount = dropCount;
			this.MaxDropCount = maxDropCount;
		}
		public override string ToString()
		{
			return string.Format("MaxDropCount : {0}, DropCount : {1}, SelfDropCount : {2}", this.MaxDropCount, this.DropCount, this.SelfDropCount);
		}
	}
}
