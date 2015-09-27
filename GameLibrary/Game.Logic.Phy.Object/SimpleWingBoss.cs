using log4net;
using SqlDataProvider.Data;
using System;
using System.Drawing;
using System.Reflection;
namespace Game.Logic.Phy.Object
{
	public class SimpleWingBoss : SimpleBoss
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		public SimpleWingBoss(int id, BaseGame game, NpcInfo npcInfo, int direction, int type) : base(id, game, npcInfo, direction, type)
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
