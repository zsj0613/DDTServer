using System;
namespace Game.Logic.Phy.Actions
{
	public class BombAction
	{
		public float Time;
		public int Type;
		public int Param1;
		public int Param2;
		public int Param3;
		public int Param4;
		public int TimeInt
		{
			get
			{
				return (int)Math.Round((double)(this.Time * 1000f));
			}
		}
		public BombAction(float time, ActionType type, int para1, int para2, int para3, int para4)
		{
			this.Time = time;
			this.Type = (int)type;
			this.Param1 = para1;
			this.Param2 = para2;
			this.Param3 = para3;
			this.Param4 = para4;
		}
	}
}
