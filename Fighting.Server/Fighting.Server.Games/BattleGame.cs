using Fighting.Server.Rooms;
using Game.Logic;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;
using System.Text;
namespace Fighting.Server.Games
{
	public class BattleGame : PVPGame
	{
		private ProxyRoom m_roomRed;
		private ProxyRoom m_roomBlue;
		public ProxyRoom Red
		{
			get
			{
				return this.m_roomRed;
			}
		}
		public ProxyRoom Blue
		{
			get
			{
				return this.m_roomBlue;
			}
		}
		public BattleGame(int id, List<IGamePlayer> red, ProxyRoom roomRed, List<IGamePlayer> blue, ProxyRoom roomBlue, Map map, eRoomType roomType, eGameType gameType, int timeType, int npcId) : base(id, roomBlue.RoomId, red, blue, map, roomType, gameType, timeType, npcId)
		{
			this.m_roomRed = roomRed;
			this.m_roomBlue = roomBlue;
		}
		public override string ToString()
		{
			return new StringBuilder(base.ToString()).Append(",class=BattleGame").ToString();
		}
		public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
		{
			Player player = base.RemovePlayer(gp, IsKick);
			if (player != null)
			{
				if (player.Team == 1)
				{
					this.m_roomRed.RemovePlayer(gp);
				}
				else
				{
					this.m_roomBlue.RemovePlayer(gp);
				}
			}
			return player;
		}
	}
}
