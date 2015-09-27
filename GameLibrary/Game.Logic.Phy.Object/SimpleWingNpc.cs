using log4net;
using SqlDataProvider.Data;
using System;
using System.Drawing;
using System.Reflection;
namespace Game.Logic.Phy.Object
{
	public class SimpleWingNpc : SimpleNpc
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public SimpleWingNpc(int id, BaseGame game, NpcInfo npcInfo, int type, int direction, int rank) : base(id, game, npcInfo, type, direction, rank)
		{
		}
		public override Point StartFalling(bool direct, int delay, int speed)
		{
			return new Point
			{
				X = this.X,
				Y = this.Y
			};
		}
	}
}
